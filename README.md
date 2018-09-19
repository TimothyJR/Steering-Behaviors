# Flocking
A project to show flocking behavior created in Unity Engine. A video of the project can be found [here](https://www.youtube.com/watch?v=WWfV0Rc7MG4)

![alt text](Images/Scene2_1.png)

Behavioral programming through the use of forces to simulate a flock seeking out a target. 

![alt text](Images/Scene3.png)

This project was implemented to test the unity job system and see it's performance benefits. Before implementing the job system, the project could manage about 135 butterflies at 80 frames per second in the Unity Editor. This was with a very simple enviornment compared to the final project. Using the same settings I wanted to test another method before going onto the job system. So I changed how the program determined to use separation force by using collision detection. This worked out better as long as the separation force had a high enough weight. But the frame rate was a lot less stable. 

So I went on to test the job system. While figuring out how to set up the job system, I realized that this project was not optimal for it because of the interactions between the flockers and how it would cause race conditions. But because of how inefficient the separation check becomes when more flockers are added, I wanted to see if there was still a performance increase. So I coded it in such a way that avoided the race conditions, but cost some performance. This resulted in managing 200 flockers at 80 frames per second. This may not seem like the biggest increase in performance, but the original code ran at single digit frames per second with 200 flockers. I then went in and made other changes and ultimately ended up settling on 400 butterflies after adding an environment and still running at an average of 60 frames per second.

![alt text](Images/Scene2_2.png)
