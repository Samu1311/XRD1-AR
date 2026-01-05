### Introduction
The XR development course was as challenging as it was exciting.

At the beginning of the project, neither I nor the rest of the group had prior experience working with Unity, with our skills in C# being limited as well. This made the course especially demanding, as we had to learn a new engine that comes with new terminology and structure, enhance our skills in a less-used programming language, and become familiar with a different development workflow simultaneously.

However, the practical nature of the course in addition to the free reign we were given with ideas, quickly made it a very engaging experience and worth the challenges.

Our project, SARA – Scannable AR Anatomy, aimed to create an AR biology visualizer that could help medical and biology students better understand complex anatomical structures. It enables them to explore 3D models in their own physical environment, as well as interact with them in various ways, making use of one of the main advantages of AR: combining real-world context with digital content.

Defining the clear purpose, as well as the fact that we could envision an application like this to be genuinely useful, also made it all the more gratifying.

### Contributions and Technical Work

My main contributions focused on implementing move, rotate, and scale interactions for AR anatomy models using touch input together with Alejandro. 

These interactions were directly tied to core user stories, e.g. allowing medical students to freely position and examine organs from different angles and distances to gain a better understanding. After all, learning has been consistenly proven to be significantly elevated through visuals and, even more so, interactions.
This also reflects how interaction design in XR notable differs from traditional applications, since the input is continuous, spatial, and closely tied to user movement rather than simple clicks or taps on the screen.

Additionally, as small of a contribution as it is, I was the one who came up with the abbreviated name of the application (hehe).

### Learning Outcomes, Challenges, and Reflection

This was our first project in Unity, and it helped us build the base of our knowledge when it comes to how AR applications are structured in Unity, as well as understanding the role of AR managers, prefabs, tracking systems, interaction scripts etc. 

The most important learning experience for me was definitely implementing the Zoom interaction. This required a different approach in thinking, planning and implementing a solution when compared to the software we had worked with before, such as webapps. Instead of reacting to discrete inputs, we had to plan for continuous input over time, and interpret user intent from physical gestures, which can be very varied and hard to predict.
Most adjustements came from our own trials and errors when trying out the app.

In order to accomplish zooming, I had to ensure manual tracking of two touch points and calculate the change in distance between them to determine whether the user intended to zoom in or out. Small snippet:

`float pinchDistance = Vector2.Distance(p0, p1);`

`float pinchDelta = pinchDistance - lastPinchDistance;`

`lastPinchDistance = pinchDistance;`

It was definitely interesting to realise how much work and thought goes behind even seemingly the simplest actions and gestures that we are so used to in our daily lives.

Another important learning point was ensuring that only the intended object responds to the gesture. 
Through achieving this, I learned about raycasting from the camera to check whether the midpoint between the two touches actually intersects with the model or one of its child objects before allowing it to be scaled. 
This helped prevent multiple objects from reacting to the same input.


When we were testing out our scripts with models, we realised that the user experience needed improvement, as scaling felt quite unstable and clumsy. Thus, we also implemented smoothing for the scaling behaviour, which made the interaction feel much more natural, visually stable and pleasant to use in an AR context:

`currentScale = Mathf.SmoothDamp(
    currentScale,
    targetScale,
    ref scaleVelocity,
    smoothTime
);`

`transform.localScale = originalScale * currentScale;`

One of the most frustrating challenges both Alejandro and I faced was debugging inconsistent interaction behaviour. 
For a long time, we assumed the issues were caused by mistakes in our logic and spent considerable time rewriting and adjusting the scripts. Eventually, we discovered that the real cause of the trouble was that we were utilising an outdated touch-input library that we took from one of the learning materials online, which was incompatible with our current setup. 

While frustrating, this became an important lesson in how choices in different tools can significantly affect development time and debugging, especially when working with unfamiliar frameworks. 
Additionally, this later helped me catch and resolve a similar issue much faster in our VR project.

However, it still meant that we had to deprioritise certain features and ideas we had, such as an 'exploded' or dissected view of the organs, where individual parts, such as the jaw or lobes of a skull, could be independently moved and rotated. 
It would have significantly increased both the educational value as well as UX, in addition to being fun to develop, so we learned the importance of being careful when selecting tools, as well as proper planning.

Overall, despite the steep learning curve and technical setbacks, this course enabled me to gain practical skills in AR development and a solid understanding of XR technologies and their use cases. Any obstacles faced throughout the project only strengthened our problem-solving abilities, while igniting a genuine interest in XR and game development.
