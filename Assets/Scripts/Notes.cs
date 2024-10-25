/*
Player
    health
        Max health
        Current health
        Damaged
        Healed
    attack (weapon)
        WeaponType.fire()
    Fly
        speed
        Stamina
    Ground move
        speed
    Interact
        Pick up weapon
        Pick up item?
    Pause
        Action? // Stuff gets player, not other way around

Player Object? // Keeps player data between scenes, maybe inventory

Weapon Collection object?

Weapon
    Name/ID
    Visual
    animator?
    Bullet Prefab
    Fire()
        // Special effect like buckshot
        Instantiate(bullet)

Bullet class
    Visual
    Animator?
    damage
    Speed
    effect
    Fire()

Interactable Abstract Class? // Maybe object?
    Obtain()

Enemy Abstract Class
    Health
    Attack
    movement

Enemy Type #X Class
    behavior

Managers:
    Main Menu Manager
    Pause Manager
    Scene manager
    Camera Manager?
    Game Manager?
    UI Manager? // Need at least this or game manager
*/

/* 
// Player
Player // Missing interact
Basic Player Bullet // Gravity, unfinished
Goo Bullet // Has crit stuff
Special Bullet // Large one
Fishing Bullet?
Uprades:
    Bounce?
    Flying speed up
    Stamina Up
    Damage Up in flight

// Enemies
Enemy Base
Temp Enemy
Enemy Bullet // Basic
Regular: 
    Spider (M)
    Ant
    Bee?
    Centipide?
Spider (F) // Boss enemy

// Misc Scripts
Bullet Base

// World Mechanics:
Door // Opens when certain enemies are dead. Unseen/offscreen. 
Camera Bounding

// Managers: 
UIManager // For health, maybe unnecessary?
Main Menu
Pause // Merge with UI probably
Sound // +extra baggage
Information Manager // idk what this entails, but its gonna be big




// Misc Stuff
Asset Implementation: // Art does this
    Import 3D assets
    Sprite assets
    Lighting
    VFX
Sound:
    Find Music
    Find SFX
Writing Implementation:
    Item descriptions
    Floating Tutorial Text
    Dialogue?
 */