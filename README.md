# Throw It!

A physics-based hyper-casual arcade game developed in Unity. The objective is simple: strategically aim your cannon to knock all the wooden and tin targets off their platforms using a limited number of projectiles.

This project was built primarily to learn, practice, and solidify my understanding of scalable game architecture, physics optimization, and "game feel" elements in Unity. While heavily inspired by popular hyper-casual mechanics, the core focus was to build everything from scratch with a clean, AAA-standard approach.

##  Gameplay Showcase

<div align="center">
  <a href="https://www.youtube.com/shorts/E4xOrgy2D5s">
    <img src="https://img.youtube.com/vi/E4xOrgy2D5s/maxresdefault.jpg" alt="Throw It Gameplay" width="400">
  </a>
  <br>
  <i>Click the image above to watch the full gameplay video on YouTube!</i>
</div>

##  Technical Architecture & Features

Rather than relying on quick fixes, this project was built using industry-standard patterns to ensure zero memory leaks, smooth 60 FPS performance, and a highly modular codebase.

* **Single-Scene State Machine:** The entire game loop (Menu -> Game -> Level Complete / Game Over) is managed by a centralized `GameManager` utilizing the `IGameStateListener` interface, eliminating tedious and performance-heavy scene reloading.
* **Zero-Gravity Impact Physics:** Targets remain frozen (gravity disabled, physics active) until the first kinetic impact. This prevents the "bouncing off a brick wall" effect and creates explosive, highly satisfying kinetic knockbacks.
* **Robust Object Pooling:** Projectiles are managed via a custom Object Pool to prevent garbage collection spikes and maintain high performance during rapid firing.
* **Observer Pattern Integration:** The `GoalManager` tracks target counts by directly querying spawned level prefabs, decoupling UI logic from the physics engine and eliminating "ghost target" bugs.
* **Game Feel ("Juice"):** Fluid UI popups, seamless screen faders, and button animations are powered by `LeanTween`. The dynamic `SoundManager` uses pitch-randomization to ensure consecutive impacts sound organic and satisfying.

##  Tech Stack

* **Engine:** Unity
* **Language:** C#
* **Architecture:** State Machine, Observer Pattern, Object Pooling, Data-Driven Level Management

##  Credits & Assets

A huge thank you to the creators of the following free assets and tools that helped bring this practice project to life.

### 3D Models & Environments
| Asset | Creator / Source | Link |
| :--- | :--- | :--- |
| **Stylish Cannon Pack** | Unluck Software | [Asset Store](https://assetstore.unity.com/packages/3d/props/weapons/stylish-cannon-pack-174145) |
| **Hyper-Casual Cartoon Castles** | 3D.rina | [Asset Store](https://assetstore.unity.com/packages/3d/props/exterior/hyper-casual-cartoon-castles-290687) |
| **Willowwood Low Poly Nature** | Distant Lands | [Asset Store](https://assetstore.unity.com/packages/3d/environments/fantasy/willowwood-free-low-poly-nature-pack-386152) |

### UI & 2D Art
| Asset | Creator / Source | Link |
| :--- | :--- | :--- |
| **Hyper Casual UI Pack** | ricimi | [Asset Store](https://assetstore.unity.com/packages/2d/gui/hyper-casual-ui-pack-375832) |
| **Seamless Grass Texture** | Magnific | [Magnific.com](https://www.magnific.com/free-vector/seamless-textured-grass-natural-grass-pattern_11930799.htm) |

### VFX & Tools
| Asset | Creator / Source | Link |
| :--- | :--- | :--- |
| **Cartoon FX Remaster Free** | Jean Moreno (JMO) | [Asset Store](https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565) |
| **LeanTween** | Dented Pixel | [Asset Store](https://assetstore.unity.com/packages/tools/animation/leantween-3595) |

### Audio (SFX)
| Sound Effect | Creator / Source | Link |
| :--- | :--- | :--- |
| **Cannon Shot** | Pixabay | [Powerful Cannon Shot](https://pixabay.com/sound-effects/horror-powerful-cannon-shot-02-487887/) |
| **Cannon Explosion** | Pixabay | [Cartoon Explosion](https://pixabay.com/sound-effects/film-special-effects-cartoon-explosion-567193/) |
| **Tin Target Knockdown** | Pixabay | [Empty Beer Can Drop](https://pixabay.com/sound-effects/film-special-effects-empty-beer-can-table-foley-drop-5-238694/) |
| **Wood Target Knockdown** | Pixabay | [Wood Smash](https://pixabay.com/sound-effects/film-special-effects-wood-smash-5-170421/) |
| **Win Sound (Success Fanfare)** | Pixabay | [Success Fanfare](https://pixabay.com/tr/sound-effects/film-ve-özel-efektler-success-fanfare-trumpets-6185/) |
| **Lose Sound (Losing Horn)** | Pixabay | [Losing Horn](https://pixabay.com/tr/sound-effects/film-ve-özel-efektler-losing-horn-313723/) |

---
*Developed as a personal project to study Unity optimization, C# patterns, and game feel.*