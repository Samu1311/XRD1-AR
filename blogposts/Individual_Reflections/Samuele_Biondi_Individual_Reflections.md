# Individual Reflection – AR Project (SARA)

## Main Contributions

- Implemented **marker-based image tracking** and model spawning using AR Foundation
- Co-developed **rotation, zoom, and reset interaction logic** with clear gesture separation
- Designed **pivot-based prefab hierarchies** to ensure correct spatial behavior
- Integrated a **second anatomical model (kidney)** using the same scalable pipeline
- Contributed to **GitHub workflow management** and collaborative development
- Participated in **pair programming and co-authored commits** due to Unity and hardware limitations

---

## Reflections

The SARA project is an educational augmented reality application that visualizes anatomical 3D models anchored to real-world image markers. The application follows a marker-based AR approach, where predefined images act as physical reference points for virtual content. This design aligns with the course focus on AR as a technology that augments existing physical materials (like textbooks or printed images) rather than replacing them.

My primary role was implementing image tracking, model spawning, and spatial registration using AR Foundation. This involved configuring tracked image libraries, ensuring that models spawned correctly when their corresponding images were detected, and maintaining alignment as the user or marker moved. I also worked extensively on XR interaction design, particularly rotation, zoom, and reset functionality, and contributed to structuring prefabs in a way that supported modularity and reuse.

From a theoretical standpoint, the project made concepts such as feature-point detection, image scoring, and registration much clearer for me. Marker-based tracking depends heavily on the visual quality of reference images, which became evident when certain images failed to track reliably. Understanding how AR systems rely on distinctive features helped guide decisions about image design and asset selection. Maintaining spatial coherence proved essential, as even small tracking instabilities immediately reduced the perceived realism and usefulness of the application.

Interaction design highlighted key differences between AR and traditional mobile applications. In AR, user input is closely tied to spatial context and camera pose, which makes interaction design more sensitive. Early implementations suffered from gesture conflicts, especially when rotation and zoom shared similar inputs. Resolving this by separating one-finger rotation from two-finger pinch zoom, and adding a reset button, significantly improved usability and reflects XR best practices discussed in the course.

Choosing image tracking over plane detection or free placement was a deliberate design decision. Image tracking provides predictable and repeatable anchors, which is particularly valuable in an educational context where specific content must correspond to specific learning material. Structuring interaction logic and annotations within prefabs ensured that each anatomical model was self-contained, which later allowed us to integrate the kidney model with minimal changes to the existing system.

Beyond what was covered in the development blog, the project emphasized how XR development is strongly influenced by hardware constraints and device testing. Many issues related to scale, tracking stability, and interaction behavior only became apparent when testing on real devices. Additionally, working in Unity as a group reinforced the importance of version control discipline, prefab organization, and clear communication, especially when resolving merge conflicts or collaborating through pair programming.

One limitation that became clear during development is the dependency of marker-based AR on environmental conditions. While image tracking provided stable and predictable anchors, it required the markers to remain visible, well-lit, and unobstructed. Occlusion, lighting changes, and viewing distance directly affected tracking quality. This reflects a key tradeoff discussed in the course: marker-based AR prioritizes robustness and simplicity over spatial flexibility. If more time had been available, I would have explored hybrid approaches (like combining image tracking with plane detection or improved occlusion handling) to increase realism while preserving stability. Additionally, extending the interaction system with features such as exploded anatomical views or context-sensitive annotations could further enhance the educational value of the application.

Overall, this project shifted my understanding of augmented reality from a graphical overlay technique to a sensor-driven spatial computing system where tracking, interaction, and perception are tightly interconnected. Working hands-on with AR Foundation, real devices, and XR-specific interaction challenges made the theoretical concepts from the course concrete and actionable. The experience not only strengthened my technical confidence in AR development but also clarified the design considerations required to build usable and meaningful XR applications, forming a strong basis for future work, specially as we moved towards more immersive VR-focused projects.

---

*Author: Samuele Biondi (316357)*
