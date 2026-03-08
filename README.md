# ZugZwang
.. chess-like planning meets endless runner pace in a mobile setting. 
Figurines move automatically, swipe to manipulate the board.

- Created in Unity Engine.
- 2 Programmers. The other one is [Jannik Kluge](https://github.com/prayyOnIntelliJ).
- Self-set Task: Create a mobile game in Unity for Google Play Store in 10 weeks.

<div align="center">
  
![SwipeGraceFeature-ezgif com-optimize](https://github.com/user-attachments/assets/64df9302-624a-4764-9545-dab271007ba4)

</div>

## How To Run
Head to Playstore: [here](https://play.google.com/store/apps/details?id=net.limitedheadspace.zugzwang)

WebGL embed at itch.io (Not the target Platform): [here](https://tom-grzembke.itch.io/zugzwang-pc-version)

# My Responsibility

<div align="center">

System | Script | Purpose | 
  --- | --- | --- | 
  CoroutineOperation | [View](https://github.com/TomGrzembke/ZugZwang/blob/main/Assets/_Workdata/GameWorld/Environment/Scripts/CoroutineOperation.cs) | Acts as a Container for a [Coroutine](https://docs.unity3d.com/530/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html) to receive an event OnFinished and cancel it early with more status info.|
AutoMove | [View](https://github.com/TomGrzembke/ZugZwang/blob/main/Assets/_Workdata/GameWorld/Character/Scripts/AutoMove.cs) | Script Responsible for the Auto Movement of the Figurines.| 
ObjectPooling | [View](https://github.com/TomGrzembke/ZugZwang/blob/main/Assets/_Workdata/Utility/Scripts/ObjectPooling.cs) | General Object Pooling System for reusing VFX, Figurines, Environment sets and Playing field segments. I adapted and tailored it to our needs from this [Tutorial](https://youtu.be/Ah3epb2HGCw).| 

</div>

# Additional Info
This Project was displayed for 5 days at the [Berlin IFA 2025](https://www.linkedin.com/posts/tom-grzembke_not-even-2-weeks-have-passed-since-gamescom-activity-7369819351832530946-s1Kl) (Trigger warning: LinkedIn).

- The primary VCS was peforce, that's why the initial commit contains most files.
- Release date: 15.08.2025
- Unity Version: 6000.0.51f1
- Team Size : 9

## Packages: 
- [Deadcows Mybox](https://github.com/Deadcows/MyBox.git) (Editor Utils and Handy Extension Methods)
- "New" Input System
- [ParticleEffectForUGUI](https://github.com/mob-sakai/ParticleEffectForUGUI)
