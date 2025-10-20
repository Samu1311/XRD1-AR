# XRD1-AR
Augmented Reality Project for XRD1 course from VIA University College (GBE Students)

# 🧬 AR Biology Visualizer

An **Augmented Reality (AR) learning tool** that allows biology and medical students to explore interactive 3D models of human anatomy.  
Using AR technology, students can **place organs in their real environment**, manipulate them (rotate, zoom, reposition), and optionally **dissect or annotate** them for deeper understanding.  

---

## ✨ Features
- 📱 **AR Model Placement** – place anatomy models on real-world surfaces.  
- 🌀 **Interactive Controls** – pinch to zoom, rotate, drag to reposition.  
- 🔎 **Exploration Mode** – toggle *explode view* to break models into parts.  
- 🏷️ **Annotations (optional)** – tap hotspots to reveal details of organ structures.  
- 🧩 **Multiple Models** – Heart, Brain, Cells, Digestive System (expandable).  

---

## 🎯 Motivation
Textbooks and 2D images make it difficult to grasp the spatial relationships of human anatomy.  
This app bridges that gap by making anatomy **immersive, manipulable, and fun to study**.  

---

## 🚀 Tech Stack
- **Unity 2022.3 LTS** with [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest/)  
- **XR Plugin Management** (ARCore for Android, ARKit for iOS)  
- **Blender** for 3D model preparation and optimization  
- **GitHub** for collaboration and version control  

---

## 🗓️ Project Roadmap (3 Weeks)

| **Week** | **Focus** |
|----------|------------|
| Week 1   | Setup Unity project, configure AR Foundation, implement plane detection & model placement. |
| Week 2   | Import first model (Heart), enable interactions (rotate, zoom, drag), implement advanced feature (explode OR annotations). |
| Week 3   | Add additional models (Brain, Cell, Digestive System), polish UI, device testing, and prepare demo build. |

---

## 📚 Documentation
- [Kickoff Document](./blogposts/01%20Exercises:%20Intro.md)
- [Full Software Requirements (Markdown)](./blogposts/02.%20Software%20Documentation.md)  
- [Original Software Requirements (PDF)](./Docs/XRD1_AR_Project_-_Software_Requirements.pdf)  

Each work session will be logged in `/blogposts` with progress updates and reflections.

---

## 👥 Team
- Eliza Smela
- Samuele Biondi
- Ginta Bilinska
- Alejandro Bautista 

---
## 📚 Project Blog Index – SARA (AR Biology Visualizer)

Follow the development journey of our Augmented Reality project through weekly documentation posts:

| # | Blog Title | Description |
|:-:|-------------|--------------|
| 01 | [**Exercises Intro**](blogposts/01_Exercises_Intro.md) | Introduction to initial XR exercises and setup. |
| 02 | [**Software Documentation**](blogposts/02_Software_Documentation.md) | Full technical documentation for the SARA project, including use cases and system requirements. |
| 03 | [**Process Start**](blogposts/03_Process_Start.md) | Project kickoff and initial implementation steps. |
| 04 | [**Process Continues**](blogposts/04_Process_Continues.md) | Further progress and first integration of AR features. |
| 05 | [**Finalization of Use Cases**](blogposts/05_Finalization_of_Use_Cases.md) | Refinement of requirements, functionalities, and early testing of core features. |
| 06 | [**SARA – Final Integrations**](blogposts/06_SARA_Final_Integrations.md) | Final stage: model integrations, rotation refinements, and final preparation for the complete AR experience. |

---

🧠 **Tip:** Each entry reflects the evolution of the project from early concept to a fully functional Augmented Reality biology visualizer.  

---

## 🛠️ Getting Started

### Prerequisites
- Unity **2022.3 LTS** or higher  
- Android device with **ARCore support** (iOS with ARKit optional)  
- Git with **Git LFS** installed (for large model files)  

### Installation
```bash
# Clone repository
git clone https://github.com/Samu1311/XRD1-AR.git

# Move into project folder
cd XRD1-AR

# Open in Unity Hub
