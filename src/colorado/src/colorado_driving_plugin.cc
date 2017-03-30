// Written By : Daniel Meltz
#define MY_GAZEBO_VER 5
// If the plugin is not defined then define it
#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include <random>


// Gazebo Libraries
#include <gazebo/gazebo.hh>
#include <gazebo/physics/physics.hh>
#include <gazebo/common/common.hh>
#include <gazebo/common/Time.hh>
#include <gazebo/transport/transport.hh>
#include <gazebo/msgs/msgs.hh>
#include <gazebo/math/gzmath.hh>
#include <gazebo/gazebo_config.h>


// ROS Communication
#include "ros/ros.h"
#include "std_msgs/Float64.h"
#include "std_msgs/Bool.h"

// Boost Thread Mutex
#include <boost/thread/mutex.hpp>

// Dynamic Configuration
#include <dynamic_reconfigure/server.h>
#include <colorado/coloradoConfig.h>
#include <boost/bind.hpp> // Boost Bind



// Maximum time delays
#define command_MAX_DELAY 0.3

#define WHEEL_EFFORT_LIMIT 100000

#define WHEELS_BASE 2.2
#define STEERING_FRICTION_COMPENSATION 2; // compensate the the daliure of reaching the angular velosity

#define WHEEL_DIAMETER 1.56
#define PI 3.14159265359

#define LINEAR_COMMAND_FILTER_ARRY_SIZE 750
#define ANGULAR_COMMAND_FILTER_ARRY_SIZE 500
#define EnginePower 1000
//#define MY_GAZEBO_VER 5

namespace gazebo
{
  
  class coloradoDrivingPlugin : public ModelPlugin
  {
    ///  Constructor
    public: coloradoDrivingPlugin() {}

    /// The load function is called by Gazebo when the plugin is inserted into simulation
    /// \param[in] _model A pointer to the model that this plugin is attached to.
    /// \param[in] _sdf A pointer to the plugin's SDF element.
  public: void Load(physics::ModelPtr _model, sdf::ElementPtr /*_sdf*/) // we are not using the pointer to the sdf file so its commanted as an option
    {
      //std::cout << "GAZEBO_VERSION = [" << GAZEBO_VERSION << "]"<<std::endl; 
      //float gazebo_ver = std::stof(GAZEBO_VERSION);
      std::cout << "My major GAZEBO VER = [" << GAZEBO_MAJOR_VERSION  << "]"<<std::endl; 
      //if (GAZEBO_MAJOR_VERSION > 2.0) std::cout << "GADOL"<<std::endl;c 
      //else std::cout << "NOT GADOL"<<std::endl; 
      // Store the pointer to the model
      this->model = _model;

      // Store the pointers to the joints
      this->left_wheel_1 = this->model->GetJoint("left_wheel_1");
      this->left_wheel_2 = this->model->GetJoint("left_wheel_2");
      this->right_wheel_1 = this->model->GetJoint("right_wheel_1");
      this->right_wheel_2 = this->model->GetJoint("right_wheel_2");
      this->streer_joint_left_1 = this->model->GetJoint("steering_joint_left_1");
      this->streer_joint_right_1 = this->model->GetJoint("steering_joint_right_1");
      this->spring_left_1 = this->model->GetJoint("spring_left_1");
      this->spring_right_1 = this->model->GetJoint("spring_right_1");
      this->spring_left_2 = this->model->GetJoint("spring_left_2");
      this->spring_right_2 = this->model->GetJoint("spring_right_2");

      
      // Starting Timers
      Linear_command_timer.Start();
      Angular_command_timer.Start();
      Breaking_command_timer.Start();
      this->Ros_nh = new ros::NodeHandle("coloradoDrivingPlugin_node");

      // Subscribe to the topic, and register a callback
      Steering_rate_sub = this->Ros_nh->subscribe("/colorado/Driving/Steering" , 100, &coloradoDrivingPlugin::On_Angular_command, this);
      Velocity_rate_sub = this->Ros_nh->subscribe("/colorado/Driving/Throttle" , 100, &coloradoDrivingPlugin::On_Linear_command, this);
            Breaking_sub = this->Ros_nh->subscribe("/colorado/Driving/Break" , 100, &coloradoDrivingPlugin::On_Break_command, this);
      
      platform_hb_pub_ = this->Ros_nh->advertise<std_msgs::Bool>("/Sahar/link_with_platform" , 100);

      // Listen to the update event. This event is broadcast every simulation iteration. 
      this->updateConnection = event::Events::ConnectWorldUpdateBegin(boost::bind(&coloradoDrivingPlugin::OnUpdate, this, _1));
      std::cout << "Setting up dynamic config"<<std::endl;

      this->model_reconfiguration_server = new dynamic_reconfigure::Server<colorado::coloradoConfig> (*(this->Ros_nh));
      this->model_reconfiguration_server->setCallback(boost::bind(&coloradoDrivingPlugin::dynamic_Reconfiguration_callback, this, _1, _2));
      std::cout << "dynamic configuration is set up"<<std::endl;
      /* initialize random seed: */
      // srand (time(NULL));
      // this->Linear_Noise_dist = new std::normal_distribution<double>(0,1);
      // this->Angular_Noise_dist = new std::normal_distribution<double>(0,1);
      Steering_Request = 0;
      Linear_command = 0;
      std::cout << "Driving Plugin Loaded"<<std::endl; 
      }

    public: void dynamic_Reconfiguration_callback(colorado::coloradoConfig &config, uint32_t level)
      {
          control_P = config.Steer_control_P;
          control_I = config.Steer_control_I;
          control_D = config.Steer_control_D;

          Steering_multiplier=config.Steering;
          damping=config.Damping;
          power=config.Power;

          suspenSpring=config.Spring;
          SuspenDamp=config.Damper;
          SuspenTarget=config.Target;

          command_lN = config.Command_Linear_Noise;
          command_aN = config.Command_Angular_Noise;
      }

    // Called by the world update start event, This function is the event that will be called every update
    public: void OnUpdate(const common::UpdateInfo & /*_info*/)  // we are not using the pointer to the info so its commanted as an option
    {
          // std::cout << "update function started"<<std::endl; 
           // std::cout << "command_timer = " << command_timer.GetElapsed().Float() << std::endl;

            // Applying effort to the wheels , brakes if no message income
            if (Linear_command_timer.GetElapsed().Float()> command_MAX_DELAY)
            {
                // Brakes
                   Linear_command = 0;
            }
            if (Breaking_command_timer.GetElapsed().Float()> command_MAX_DELAY)
            {
                // Brakes
                   Break = 1;
            }
            if (Angular_command_timer.GetElapsed().Float()> command_MAX_DELAY)
            {
                // Brakes
                   Steering_Request = 0;
            }
            // std::cout << "Applying efforts"<<std::endl; 
            
              apply_efforts();
              ApplySuspension();
              breaker(Break);

              std::cout << this->spring_right_1->GetAngle(0).Radian()<<std::endl;
              std_msgs::Bool connection;
              connection.data = true;
              platform_hb_pub_.publish(connection);
    }
    double WheelThrottle=0;
    double DesiredAngle=0;
    private: void wheel_controller(physics::JointPtr wheel_joint, double Throttle)
    {    
        WheelThrottle=WheelThrottle+0.001*(Throttle-WheelThrottle);
        
        double wheel_omega = wheel_joint->GetVelocity(0);
        double effort_command = power*WheelThrottle - damping*wheel_omega;
      
                wheel_joint->SetForce(0,effort_command);       
    }
    private: void steer_controller(physics::JointPtr steer_joint, double Angle)
    {
        // std::cout << " getting angle"<< std::endl;
        if(steer_joint)
        {
          DesiredAngle=DesiredAngle+Steering_multiplier*(Angle-DesiredAngle);
          double wheel_angle = steer_joint->GetAngle(0).Radian();
          double steer_omega = steer_joint->GetVelocity(0);
          double effort_command = control_P*(0.6*DesiredAngle - wheel_angle)-control_D*(steer_omega);
          steer_joint->SetForce(0,effort_command); 
          //  std::cout << wheel_angle<< std::endl;
        }
          else
          std::cout << "Null Exception! \n";
        // std::cout << "efforting"<< std::endl;
        // this->jointController->SetJointPosition(steer_joint, Angle*0.61);

    }



  private: void apply_efforts()
    {
        float WheelTorque = Linear_command*EnginePower;
        // std::cout << " Controlling wheels"<< std::endl;
        wheel_controller(this->left_wheel_1 , WheelTorque);
        wheel_controller(this->left_wheel_2 , WheelTorque);

        wheel_controller(this->right_wheel_1, WheelTorque);
        wheel_controller(this->right_wheel_2, WheelTorque);
        // std::cout << " Controlling Steering"<< std::endl;
        steer_controller(this->streer_joint_left_1, Steering_Request);
        steer_controller(this->streer_joint_right_1, Steering_Request);
        // std::cout << " Finished applying efforts"<< std::endl;
    }
    private: void breaker(int breaking)
    {
      
        // std::cout << " getting angle"<< std::endl;
        if(breaking>=0.09&&!Breaks)
        {
          TempDamping=damping;
          damping=10000*Break*Break;
          Breaks=true;
          // std::cout << "Break on "<<damping<<std::endl; 
        }
          else if(breaking==0&&Breaks)
          {
            damping=TempDamping;
            Breaks=false;
            // std::cout << "Breaks off "<<damping<<std::endl; 
          }
          if(breaking>=0.09&&Breaks) damping=1000*Break*Break;
         
        // std::cout << "efforting"<< std::endl;
        // this->jointController->SetJointPosition(steer_joint, Angle*0.61);

    }

    private: void On_Break_command(const std_msgs::Float64ConstPtr &msg)
    {
     Breaking_command_mutex.lock();
          // Recieving referance velocity
          if(msg->data >= 1) Break = 1;
           else if(msg->data >= 0.09) Break = msg->data;
          else                     Break = 0;
          


        // Reseting timer every message
#if GAZEBO_MAJOR_VERSION >= 5
           Breaking_command_timer.Reset();
#endif
           Breaking_command_timer.Start();

      Breaking_command_mutex.unlock();
    }
    // The subscriber callback , each time data is published to the subscriber this function is being called and recieves the data in pointer msg
    private: void On_Angular_command(const std_msgs::Float64ConstPtr &msg)
    {
      Angular_command_mutex.lock();
          // Recieving referance steering angle  
          if(msg->data > 1)       { Steering_Request =  1;          }
          else if(msg->data < -1) { Steering_Request = -1;          }
          else                    { Steering_Request = msg->data;   }

        // Reseting timer every time LLC publishes message
#if GAZEBO_MAJOR_VERSION >= 5 
           Angular_command_timer.Reset();
#endif
           Angular_command_timer.Start();


      Angular_command_mutex.unlock();
    }
    
    private: void ApplySuspension()
    {
      Suspension(spring_left_1);
      Suspension(spring_left_2);
      Suspension(spring_right_1);
      Suspension(spring_right_2);
    }
    private: void Suspension(physics::JointPtr Suspension)
    {
      double Force = -(Suspension->GetAngle(0).Radian()+SuspenTarget)*suspenSpring - Suspension->GetVelocity(0)*SuspenDamp;
            Suspension->SetForce(0, Force);
    }


    // The subscriber callback , each time data is published to the subscriber this function is being called and recieves the data in pointer msg
    private: void On_Linear_command(const std_msgs::Float64ConstPtr &msg)
    {
      
      Linear_command_mutex.lock();
          // Recieving referance velocity
          if(msg->data > 1)       { Linear_command =  1;          }
          else if(msg->data < -1) { Linear_command = -1;          }
          else                    { Linear_command = msg->data;   }

        // Reseting timer every time LLC publishes message
#if GAZEBO_MAJOR_VERSION >= 5
           Linear_command_timer.Reset();
#endif
           Linear_command_timer.Start();

      Linear_command_mutex.unlock();
    }




     private: float gazebo_ver;

     // Defining private Pointer to model
     private: physics::ModelPtr model;

      // Defining private Pointer to joints

     private: physics::JointPtr right_wheel_1;
     private: physics::JointPtr right_wheel_2;
     private: physics::JointPtr left_wheel_1;
     private: physics::JointPtr left_wheel_2;
     private: physics::JointPtr streer_joint_left_1;
     private: physics::JointPtr streer_joint_right_1;
     private: physics::JointPtr spring_left_1 ;
     private: physics::JointPtr spring_right_1;
     private: physics::JointPtr spring_left_2 ;
     private: physics::JointPtr spring_right_2;



      // Defining private Pointer to the update event connection
     private: event::ConnectionPtr updateConnection;

     // Defining private Ros Node Handle
     private: ros::NodeHandle  *Ros_nh;

     // Defining private Ros Subscribers
     private: ros::Subscriber Steering_rate_sub;
     private: ros::Subscriber Velocity_rate_sub;
     private: ros::Subscriber Breaking_sub;
     // Defining private Ros Publishers
     ros::Publisher platform_hb_pub_;


     // Defining private Timers
     private: common::Timer Linear_command_timer;
     private: common::Timer Angular_command_timer;
     private: common::Timer Breaking_command_timer;


     // Defining private Mutex
     private: boost::mutex Angular_command_mutex;
     private: boost::mutex Linear_command_mutex;
     private: boost::mutex Breaking_command_mutex;
     private: float Linear_command;
     private: float Steering_Request;
     private: int Break=1;
     private: double Linear_ref_vel;
     private: double Angular_ref_vel;

     private: bool Breaks=false;

     private: double Linear_command_array[LINEAR_COMMAND_FILTER_ARRY_SIZE];
     private: double Angular_command_array[ANGULAR_COMMAND_FILTER_ARRY_SIZE];
     private: double Linear_command_sum;
     private: double Angular_command_sum;
     private: int Linear_command_index;
     private: int Angular_command_index;


     private: dynamic_reconfigure::Server<colorado::coloradoConfig> *model_reconfiguration_server;
     private: double control_P, control_I ,control_D,Steering_multiplier,damping,power, TempDamping,suspenSpring, SuspenDamp, SuspenTarget;		// PID constants
     private: double command_lN, command_aN;   // command noise factors

    //  std::default_random_engine generator;
    //  std::normal_distribution<double> * Linear_Noise_dist;
    //  std::normal_distribution<double> * Angular_Noise_dist;

  };

  // Tell Gazebo about this plugin, so that Gazebo can call Load on this plugin.
  GZ_REGISTER_MODEL_PLUGIN(coloradoDrivingPlugin)
}

