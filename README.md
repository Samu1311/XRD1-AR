# XRD1-AR
Augmented Reality Project for XRD1 course from VIA University College (GBE Students)

AR Biology Visualizer 🫀🧠🫁

## 1. Overview
The **AR Biology Visualizer** is a mobile AR application designed to support biology students in exploring complex human anatomy.  
By placing interactive 3D organ models (e.g., heart, brain, cells, digestive system) in their environment, students gain a hands-on learning experience that goes beyond textbooks and 2D diagrams.  

**Team:** Eliza, Samuele, Ginta, Alejandro  
**Duration:** 3 weeks  
**Tools:** Unity + AR Foundation, Blender (for 3D models), GitHub (collaboration & documentation)

---

## 2. Problem Statement
Medical and biology students often struggle to understand the spatial relationships of anatomical structures using traditional 2D resources.  
**AR** solves this by making anatomy immersive, interactive, and explorable at scale.  

---

## 3. Core Requirements
- Place anatomy models on real-world surfaces.  
- Interact with models (rotate, zoom, reposition).  
- Switch between multiple organ models.  
- Provide at least one advanced feature:  
  - Explode view (dissect into parts), **or**  
  - Hotspot annotations with descriptions.  

---

## 4. Use Cases (Examples)
- **UC1:** Student places a 3D heart model on a desk.  
- **UC2:** Student rotates and zooms the model for examination.  
- **UC3:** Student toggles “Explode View” to see internal parts.  
- **UC4:** Student taps hotspots to read descriptions of organ parts.  

---

## 5. Roadmap (3 Weeks)

| **Week** | **Focus** |
|----------|------------|
| Week 1   | Project setup, AR Foundation config, plane detection, placement, basic interactions (rotate/zoom/drag). |
| Week 2   | First model integration (Heart). Implement advanced feature (explode OR annotations). |
| Week 3   | Add more models (Brain, Cell, Digestive System). Polish interactions & UI. Final testing and demo build. |

---

## 6. Collaboration Workflow
- Repo setup with **Git LFS** for large models.  
- Branching strategy:  
  - `main` → stable builds  
  - `dev` → integration  
  - `feature/*` → feature branches  
- Documentation: new Markdown post after each work session in `/blogposts`.  
- Issues/Kanban: track tasks and progress on GitHub.  

---

## 7. References
- Full [Software Requirements Document (Markdown)](./Software_Requirements.md)  
- Original [Software Requirements PDF](./XRD1_AR_Project_-_Software_Requirements.pdf)  
