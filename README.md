<div align="center">

<!-- Gradient Visual Header -->
<img width="100%" src="https://capsule-render.vercel.app/api?type=waving&height=280&color=gradient&text=DROOB%20-%20JOB%20PLATFORM&fontSize=55&fontAlignY=38&animation=fadeIn&fontColor=ffffff"/>

<!-- Professional Animated Typing Description -->
<img src="https://readme-typing-svg.herokuapp.com?font=JetBrains+Mono&weight=700&size=24&duration=2500&pause=800&color=58A6FF&center=true&vCenter=true&width=1000&lines=AI-Powered+Recruitment+Platform;Built+with+ASP.NET+Core+%7C+React;Clean+Architecture+%26+DDD+Practitioner;Real-Time+Talent+Matching+System"/>

<br>

[![Platform Status](https://img.shields.io/badge/Stage-Production--Ready-success?style=for-the-badge)](#)
[![Backend Ecosystem](https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge)](#)
[![Frontend Framework](https://img.shields.io/badge/React-18.x-blue?style=for-the-badge)](#)

</div>

---

# 📌 Project Vision & Abstract

**Droob (JobPlatform)** is an intelligent, enterprise-grade recruitment platform designed to revolutionize how talent connects with employers. Moving beyond conventional job boards, Droob integrates **Artificial Intelligence (AI)** to analyze Resumes/CVs, generate objective alignment scoring, and construct personalized learning roadmaps. Backed by a high-performance messaging infrastructure, it bridges the gap between hiring managers and applicants instantly.

---

# 🏗️ Architectural Pattern: Clean Architecture

The core system is engineered following **Clean Architecture** and Domain-Driven Design (DDD) guidelines. This structure enforces a strict separation of concerns, isolating business rules from data access frameworks and external third-party integrations:

```text
📦 Droob.Backend
 ├── 🏛️ Droob.Domain
 │    └── Entities, Value Objects, Enterprise Rules, Aggregate Roots
 ├── ⚙️ Droob.Application
 │    └── Use Cases, CQRS Handlers, Interfaces, DTOs, Mapping
 ├── 🔌 Droob.Infrastructure
 │    └── Data Access (EF Core, SQL Server), Repositories, Unit of Work, Gemini AI Service
 └── 💻 Droob.WebAPI
      └── Controllers, SignalR Hubs, Middlewares, Configurations
