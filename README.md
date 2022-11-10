# Team11_42_MicroBosch
Calculating the Driving metric score through Jerk

Hack Challenge #2 : A solution for Safety features of Drivers with cloud connected intervehicle Data.

##Probable Problematic Scenario: 
![image](https://user-images.githubusercontent.com/66947064/200791616-d98f1f71-f220-4858-8318-ebbe80ddfa49.png)
                  ![Uploading image.png…]()
                   ![image](https://user-images.githubusercontent.com/66947064/200791236-733d759e-a4e8-43d4-bb5b-f9283a8dbf03.png)


#Our Probable Solution : 
            
            
              1. A Recommendation system based on the score alerting the driver to change their speeding / acceleration
              2. Metric scores can be sent to Insurance companies / other places to have a check on the Insurance risk analysis
              3. Metric scores shown to the driver with gamified level can alert them to take a more safety option
              4. Driving scores from jerk used to identify as Emotion meters 
              
  #Probable Failure Point / Challenges Faced: 
            1. Interaction between Muto and Kuksa client
            2. CAN data we got processed with computer generated data and timestamps
            3. Only speed value cannot help interpreting the relations.
            4. MQTT subscription was not clear with its annotations. So we used gRPC protocol
