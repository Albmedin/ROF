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