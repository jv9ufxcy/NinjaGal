using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.XR;
using System;

public class CharacterObject : MonoBehaviour, IHittable
{
    [Header("Movement")]
    public Vector2 velocity;

    public float aniMoveSpeed;

    public Vector3 friction = new Vector3(0.95f, 0.99f, 0.95f);
    [SerializeField] private float direction = 1;

    public Rigidbody2D myRB;
    [HideInInspector] public BoxCollider2D boxCollider2D;
    [HideInInspector] public Controller2D controller;
    [HideInInspector] public HealthManager healthManager;
    [HideInInspector] public AudioManager audioManager;
    

    [Header("CurrentState")]
    public int currentState;
    public float currentStateTime;
    public float prevStateTime;

    [Header("CharacterModel")]
    public CharacterObject characterObject;
    public GameObject character;
    public GameObject draw;
    public Animator characterAnim;
    public SpriteRenderer spriteRend;
    public Material defaultMat, whiteMat;
    public Shuriken shuriken;
    public Color flashColor = new Color ( 0,0.5f,0.75f,1f);
    public enum ControlType { AI, PLAYER, BOSS, DEAD, OBJECT };
    public ControlType controlType;

    [Header("HitCancel")]
    public Hitbox hitbox;
    public bool canCancel;
    public bool isHit;
    public int hitConfirm;

    [HideInInspector] public int keyCount;

    public InputBuffer inputBuffer = new InputBuffer();

    private Vector2 facingDir;

    // Use this for initialization
    void Awake()
    {
        myRB = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        controller = GetComponent<Controller2D>();
        spriteRend = characterAnim.gameObject.GetComponent<SpriteRenderer>();
        healthManager = GetComponent<HealthManager>();
        
        
    }
    void Start()
    {
        defaultMat = spriteRend.material;

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No Audio Manager in Scene");
        }
    }

    // Update is called once per frame
    private void Update()
    {

        switch (controlType)
        {
            case ControlType.AI:
                isNearPlayer = Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) <= aggroRange;
                isLongRange = Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) <= longAttackRange &&
                    Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) > shortAttackRange;
                isShortRange = Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) <= shortAttackRange;
                break;
            case ControlType.BOSS:
                isNearPlayer = Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) <= aggroRange;
                isLongRange = Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) <= longAttackRange &&
                    Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) > shortAttackRange;
                isShortRange = Vector3.Distance(transform.position, GameEngine.gameEngine.mainCharacter.transform.position) <= shortAttackRange;
                break;
            case ControlType.PLAYER:
                //pauseMenuCheck
                //if (Input.GetButtonDown(GameEngine.coreData.rawInputs[8].name))
                //    PauseManager.pauseManager.PauseButtonPressed();

                leftStick = new Vector2(Input.GetAxis(GameEngine.coreData.rawInputs[13].name), Input.GetAxis(GameEngine.coreData.rawInputs[14].name));
              break;
            default:
                break;
        }
    }
    void FixedUpdate()
    {
        if (!PauseManager.IsGamePaused /*&& !DialogueManager.instance.isDialogueActive*/)
        {
            if (GameEngine.hitStop <= 0)
            {
                switch (controlType)
                {
                    case ControlType.AI:
                        UpdateAI();
                        break;
                    case ControlType.BOSS:
                        UpdateAI();
                        break;
                    case ControlType.PLAYER:
                        UpdateInput();
                        break;
                    default:
                        break;
                }                
                UpdateState(); //Update State Machine        
                UpdatePhysics();//Update Physcis
            }
            UpdateTimers();
        }
       UpdateAnimator();
    }

    public void UpdateCharacter()
    {
        UpdateState();
        UpdatePhysics();
        UpdateTimers();
        UpdateAnimator();
    }
    void UpdateTimers()
    {
        if (dashCooldown > 0) { dashCooldown -= dashCooldownRate; }
        if (invulCooldown > 0) { invulCooldown --; }
        else { isInvulnerable = false; curComboValue = -1; }
    }
    [HideInInspector] public float animSpeed;
    void UpdateAnimator()
    {
        animSpeed = 1;
        if (GameEngine.hitStop > 0)
        {
            animSpeed = 0;
        }

        Vector2 latSpeed = velocity;
        aniMoveSpeed = Vector3.SqrMagnitude(latSpeed);
        characterAnim.SetFloat("moveSpeed", aniMoveSpeed);
        if (leftStick != Vector2.zero)
        {
            characterAnim.SetFloat("moveX", facingDir.x);
            characterAnim.SetFloat("moveY", facingDir.y);
        }
        characterAnim.SetFloat("hitAnimX", curHitAnim.x);
        characterAnim.SetFloat("hitAnimY", curHitAnim.y);
        characterAnim.SetFloat("animSpeed", animSpeed);

    }

    void FaceStick()//flip hitbox
    {
        if (leftStick != Vector2.zero)
            facingDir = leftStick;

        if (facingDir.y < 0)
            hitbox.transform.parent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (facingDir.y > 0)
            hitbox.transform.parent.transform.localRotation = Quaternion.Euler(0, 0, 180);
        else if (facingDir.x < 0)
            hitbox.transform.parent.transform.localRotation = Quaternion.Euler(0, 0, -90);
        else if (facingDir.x > 0)
            hitbox.transform.parent.transform.localRotation = Quaternion.Euler(0, 0, 90);

        if (CheckVelocityDeadZone())
        {
            //direction
        }
    }

    void UpdateState()
    {
        CharacterState myCurrentState = GameEngine.coreData.characterStates[currentState];

        if (hitStun > 0 && controlType!=ControlType.DEAD)  { GettingHit();  }
        else
        {
            UpdateStateEvents();
            UpdateStateAttacks();

            prevStateTime = currentStateTime;
            currentStateTime++;

            if (currentStateTime >= myCurrentState.length)
            {
                if (myCurrentState.loop) { LoopState(); }
                else { EndState(); }
            }
        }
    }
    void LoopState()
    {
        currentStateTime = 0;
        prevStateTime = -1;
    }

    void EndState()
    {
        currentStateTime = 0;
        currentState = 0;
        prevStateTime = -1;
        StartState(currentState);
    }

    void UpdateStateEvents()
    {
        int _curEv = 0;
        foreach (StateEvent _ev in GameEngine.coreData.characterStates[currentState].events)
        {
            if (_ev.active)
            {
                if (currentStateTime >= _ev.start && currentStateTime <= _ev.end)
                {
                    DoEventScript(_ev.script, currentState, _curEv, _ev.parameters);
                }
            }
            _curEv++;
        }
    }

    [Header("CurrentAttack")]
    public float hitActive;
    public int currentAttackIndex;
    void UpdateStateAttacks()
    {
        int _cur = 0;
        foreach (AttackEvent _atk in GameEngine.coreData.characterStates[currentState].attacks)
        {
            if (currentStateTime == _atk.start)
            {
                hitbox.RestoreGetHitBools();
                hitActive = _atk.length;
                hitbox.transform.localScale = _atk.hitBoxScale;
                hitbox.transform.localPosition = _atk.hitBoxPos;
                currentAttackIndex = _cur;
            }
            if (currentStateTime == _atk.start + _atk.length)
            {
                hitActive = 0;
            }
            //HitCancel
            float cWindow = _atk.start + _atk.cancelWindow;
            if (currentStateTime >= cWindow)
                if (hitConfirm > 0)
                    canCancel = true;
            _cur++;
        }
    }
    public static float whiffWindow = 8f;
    void DoEventScript(int _index, int _actIndex, int _evIndex, List<ScriptParameters> _params)
    {
        if (_params == null) { return; }
        if (_params.Count <= 0) { return; }
        switch (_index)//index = element in characterscripts
        {
            case 0:
                VelocityY(_params[0].val);
                break;
            case 1:
                FrontVelocity(_params[0].val);
                break;
            case 3:
                StickMove(_params[0].val);
                break;
            case 4:
                GettingHit();
                break;
            case 5:
                GlobalPrefab(_params[10].val, _actIndex, _evIndex);
                break;
            case 6:
                CanCancel(_params[0].val);
                break;
            case 7:
                ThrowShuriken(_params[0].val);
                break;
            case 8:
                FaceStick();
                break;
            case 9:
                FireBullet(_params[0].val, _params[1].val, _params[2].val, _params[3].val, _params[4].val, _params[5].val, _params[6].val, _params[7].val);
                break;
            case 10:
                FireArrow(_params[0].val, _params[1].val, _params[2].val, _params[3].val, _params[4].val);
                break;
            case 11:
                audioManager.PlaySound(_params[0].name);
                break;
            case 12:
                CinemachineShake.instance.ShakeCamera(_params[0].val, _params[1].val);
                break;
            case 13:
                ToggleMovelist();
                break;
            case 14:
                QuickChangeForm(formIndex);
                break;
        }
    }
    public int formIndex, maxIndex=3;
    public void ToggleMovelist()
    {
        formIndex++;
        if (formIndex > maxIndex)
        {
            formIndex = 1;
        }
    }
    public void QuickChangeForm(int index)
    {
        GameEngine.gameEngine.ChangeMovelist(index);
        Debug.Log("Form " + index);
        //characterAnim.runtimeAnimatorController = formAnims[GameEngine.gameEngine.globalMovelistIndex];
    }

    float[] rotations;
    public GameObject[] bullets;
    private void FireBullet(float bulletResource, float numberOfBullets, float speed, float velocityX, float velocityY, float minRot, float maxRot, float isRandom)
    {
        rotations = new float[(int)numberOfBullets];
        Vector2 velocity = new Vector2(velocityX, velocityY);

        if (numberOfBullets > 1)
        {
            if (isRandom != 0)
                RandomRotations((int)numberOfBullets, minRot, maxRot);
            else
                DistributedRotations((int)numberOfBullets, minRot, maxRot);
        }
        else
        {
            for (int i = 0; i < numberOfBullets; i++)
            {
                rotations[i] = hitbox.transform.parent.transform.localEulerAngles.z;
            }
        }
        SpawnBullets((int)bulletResource, (int)numberOfBullets, speed, velocity);
    }
    private void FireArrow(float bulletResource, float numberOfBullets, float speed, float minRot, float maxRot)
    {

        minRot = hitbox.transform.parent.transform.localEulerAngles.z-minRot;
        maxRot = hitbox.transform.parent.transform.localEulerAngles.z+maxRot;
        //minRot = hitbox.transform.parent.transform.localRotation.z-minRot;
        //maxRot = hitbox.transform.parent.transform.localRotation.z+maxRot;

        FireBullet(bulletResource, numberOfBullets, speed, 0, -1, minRot, maxRot, 0);
    }
    // Select a random rotation from min to max for each bullet
    public float[] RandomRotations(int numberOfBullets, float minRotation, float maxRotation)
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            rotations[i] = UnityEngine.Random.Range(minRotation, maxRotation);
        }
        return rotations;

    }

    // This will set random rotations evenly distributed between the min and max Rotation.
    public float[] DistributedRotations(int numberOfBullets, float minRotation, float maxRotation)
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            var fraction = (float)i / ((float)numberOfBullets - 1);
            var difference = maxRotation - minRotation;
            var fractionOfDifference = fraction * difference;
            rotations[i] = fractionOfDifference + minRotation; // We add minRotation to undo Difference
        }
        foreach (var r in rotations) print(r);
        return rotations;
    }
    public GameObject[] SpawnBullets(int bulletResource, int numberOfBullets, float bulletSpeed, Vector2 bulletVelocity )
    {
        // Spawn Bullets
        GameObject[] spawnedBullets = new GameObject[numberOfBullets];
        for (int i = 0; i < numberOfBullets; i++)
        {
            spawnedBullets[i] = Instantiate(bullets[bulletResource], transform.position, Quaternion.identity);

            var b = spawnedBullets[i].GetComponent<Bullet>();
            b.rotation = rotations[i];
            b.speed = bulletSpeed;
            b.velocity = bulletVelocity;
            b.character = this;
        }
        return spawnedBullets;
    }
    private void ThrowShuriken(float val)
    {
        if (shuriken.IsEquipped())
        {
            Vector3 throwDir = facingDir.normalized * val;
            shuriken.ThrowShuriken(throwDir);
        }
        else
            shuriken.Recall();
    }

    public void SetState(int stateIndex)
    {
        StartState(stateIndex);
    }
    void CanCancel(float _val)
    {
        if (_val > 0)
        {
            canCancel = true;
        }
        else
            canCancel = false;
    }
    void GlobalPrefab(float _index, int _act, int _ev)
    {
        GameEngine.GlobalPrefab((int)_index, character, _act, _ev);
    }
    public void GlobalPrefab(float _prefab)
    {
        GlobalPrefab(_prefab, -1, -1);
    }
    public void FrontVelocity(float _pow)
    {
        velocity = _pow * facingDir;
    }
    [Header("MovementVectors")]
    public Vector2 leftStick;

    void StickMove(float _pow)
    {
        //move
        velocity = leftStick.normalized*(_pow*moveSpeed);
        if (hitStun <= 0)
        {
            FaceStick();
        }
    }

    void VelocityY(float _pow)
    {
        velocity.y = _pow;
    }

    public float deadzone = 0.2f;

    public float moveSpeed = 10f;
    public void StartStateFromScript(int _newState)
    {
        StartState(_newState);
    }
    void StartState(int _newState)
    {
        currentState = _newState;
        prevStateTime = -1;
        currentStateTime = 0;
        canCancel = false;

        if (_newState == 0) { currentCommandStep = 0; }

        //Attacks
        hitActive = 0;
        hitConfirm = 0;

        SetAnimation(GameEngine.coreData.characterStates[currentState].stateName);
        //Debug.Log("State Started: " + GameEngine.coreData.characterStates[currentState].stateName);
    }
    void SetAnimation(string animName)
    {
        characterAnim.CrossFadeInFixedTime(animName, GameEngine.coreData.characterStates[currentState].blendRate);
    }

    public int currentCommandState;
    public int currentCommandStep;

    public void GetCommandState()
    {
        currentCommandState = 0;
        for (int c = 0; c < GameEngine.gameEngine.CurrentMoveList().commandStates.Count; c++)
        {
            CommandState s = GameEngine.gameEngine.CurrentMoveList().commandStates[c];
        }
    }

    int[] cancelStepList = new int[2];

    void UpdateInput()
    {
        inputBuffer.Update();

        bool startState = false;

        GetCommandState();
        CommandState comState = GameEngine.gameEngine.CurrentMoveList().commandStates[currentCommandState];

        if (currentCommandStep >= comState.commandSteps.Count) { currentCommandStep = 0; }

        cancelStepList[0] = currentCommandStep;//base sub-state
        cancelStepList[1] = 0;
        int finalS = -1;
        int finalF = -1;
        int currentPriority = -1;
        for (int s = 0; s < cancelStepList.Length; s++)
        {
            if (comState.commandSteps[currentCommandStep].strict && s > 0) { break; }
            if (!comState.commandSteps[currentCommandStep].activated) { break; }

            for (int f = 0; f < comState.commandSteps[cancelStepList[s]].followUps.Count; f++)// (CommandStep cStep in comState.commandSteps[currentCommandStep])
            {
                CommandStep nextStep = comState.commandSteps[comState.commandSteps[cancelStepList[s]].followUps[f]];
                InputCommand nextCommand = nextStep.command;

                if (CheckInputCommand(nextCommand))
                {
                    if (canCancel)
                    {
                        if (GameEngine.coreData.characterStates[nextCommand.state].ConditionsMet(this))
                        {
                            if (nextStep.priority > currentPriority)
                            {
                                currentPriority = nextStep.priority;
                                startState = true;
                                finalS = s;
                                finalF = f;

                            }
                        }
                    }
                }
            }
        }
        if (startState)
        {
            CommandStep nextStep = comState.commandSteps[comState.commandSteps[cancelStepList[finalS]].followUps[finalF]];
            InputCommand nextCommand = nextStep.command;
            inputBuffer.UseInput(nextCommand.input);
            if (nextStep.followUps.Count > 0) { currentCommandStep = nextStep.idIndex; }
            else { currentCommandStep = 0; }
            StartState(nextCommand.state);
        }
    }
    public bool CheckInputCommand(InputCommand _in)
    {
        if (inputBuffer.buttonCommandCheck[_in.input] < 0) { return false; }
        if (inputBuffer.motionCommandCheck[_in.motionCommand] < 0) { return false; }
        return true;
    }
    public bool CheckVelocityDeadZone()
    {
        if (velocity.x > 0.001f) { return true; }
        if (velocity.x < -0.001f) { return true; }
        if (velocity.y > 0.001f) { return true; }
        if (velocity.y < -0.001f) { return true; }
        return false;
    }
   
    private static void Screenshake()
    {
        Camera.main.transform.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
    }

    [Header("Timers")]
    public float invulCooldown, invulFlickerRate = 4f, dashCooldown, dashCooldownRate = 1f;
    public float specialMeter, specialMeterMax = 100f, nextSpecialMeterUse;

    public void UseMeter(float _val)
    {
        ChangeMeter(-_val);
    }
    public void BuildMeter(float _val)
    {
        ChangeMeter(_val);
    }
    public void ChangeMeter(float _val)
    {
        specialMeter += _val;
        specialMeter = Mathf.Clamp(specialMeter, 0f, specialMeterMax);
        //healthManager.ChangeMeter((int)_val);
    }
    void UpdatePhysics()
    {
        Move(velocity);
        velocity.Scale(friction);
    }
    public void Move(Vector2 velocity)
    {
        //Debug.Log("Velocity: "+velocity.ToString());
        myRB.MovePosition(myRB.position + velocity * moveSpeed * Time.deltaTime);
    }

    public void SetVelocity(Vector3 v)
    {
        velocity = v;
    }
    [Header("Hit Stun")]
    public Vector2 curHitAnim;
    public Vector2 targetHitAnim;
    private int curComboValue;

    public bool CanBeHit(AttackEvent curAtk)
    {
        if (controlType == ControlType.DEAD)
            return false;
        if (invulCooldown > 0)
        {
            if (curComboValue < curAtk.comboValue)
                return true;
            else
                return false;
        }
        else
        {
            isInvulnerable = false;
            spriteRend.color = Color.white;
            return true;
        }
    }
    public void Hit(CharacterObject attacker, int projectileIndex, int atkIndex, DamageType element)
    {
        GetHit(attacker, projectileIndex, atkIndex, element);
    }
    public DamageType[] weaknesses;
    public void GetHit(CharacterObject attacker, int projectileIndex, int atkIndex, DamageType element)
    {
        AttackEvent curAtk;
        if (projectileIndex == 0)//not a projectile
        {
            curAtk = GameEngine.coreData.characterStates[attacker.currentState].attacks[attacker.currentAttackIndex];
        }
        else//projectiles
        {
            curAtk = GameEngine.coreData.characterStates[projectileIndex].attacks[atkIndex];
        }
        if (CanBeHit(curAtk))
        {
            Vector3 nextKnockback = curAtk.knockback;
            Vector3 knockOrientation = transform.position - attacker.transform.position;
            knockOrientation.Normalize();
            nextKnockback.x *= knockOrientation.x;
            nextKnockback.y *= knockOrientation.y;
            float nextHitStun = curAtk.hitStun;
            int nextDamage = curAtk.damage;
            curComboValue = curAtk.comboValue;
            StartInvul(curAtk.hitStop);

            if (element!=null)
            {
                for (int i = 0; i < weaknesses.Length; i++)
                {
                    if (weaknesses[i]==element)
                    {
                        nextKnockback *= 2;
                        nextHitStun *= 2;
                        nextDamage *= 2;
                        element.SpawnPrefabEffect(transform.position);
                        Debug.Log("Crit");
                    }
                }
            }
            //healthManager.PoiseDamage(curAtk.poiseDamage);
            //if (healthManager.currentPoise <= 0)
            //{
                SetVelocity(nextKnockback /** 0.7f*/);//dampen a bit
                targetHitAnim.x = curAtk.hitAnim.x;
                targetHitAnim.y = curAtk.hitAnim.y;

                curHitAnim = targetHitAnim * .25f;

                hitStun = nextHitStun;
                StartState(hitStunStateIndex);
            //}

            GameEngine.SetHitPause(curAtk.hitStop);
            attacker.hitConfirm += 1;
            attacker.BuildMeter(curAtk.meterGain);
            //remove health
            healthManager.RemoveHealth(nextDamage);
            //play sound
            
            switch (controlType)
            {
                case ControlType.AI:
                    audioManager.PlaySound("HitEnemy");
                    break;
                case ControlType.PLAYER:
                    audioManager.PlaySound("HitPlayer");
                    break;
                case ControlType.BOSS:
                    audioManager.PlaySound("HitEnemy");
                    break;
                default:
                    break;
            }
        }
    }
    private bool isInvulnerable;
    private void StartInvul(float hitFlash)
    {
        if (invulCooldown <= 0 && (controlType != ControlType.AI && controlType != ControlType.OBJECT))
        {
            invulCooldown = 90f;
            isInvulnerable = true;
        }
        StartCoroutine(FlashWhiteDamage(hitFlash));
    }

    private IEnumerator FlashWhiteDamage(float hitFlash)
    {
        spriteRend.material = defaultMat;
        spriteRend.material = whiteMat;
        for (int i = 0; i < hitFlash; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        spriteRend.material = defaultMat;
        StartCoroutine(BlinkWhileInvulnerableCoroutine());
    }
    private IEnumerator BlinkWhileInvulnerableCoroutine()
    {
        while (isInvulnerable)
        {
            //yield return new WaitForSeconds(blinkInterval);
            spriteRend.color = flashColor;
            for (int i = 0; i < invulFlickerRate; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            spriteRend.color = Color.white;
            for (int i = 0; i < invulFlickerRate; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    [SerializeField] string[] attackStrings;

    [Tooltip("hitstun index in coreData")]
    public int hitStunStateIndex = 7, deathStateIndex = 36;//hitstun state in coreData
    public float hitStun;
    public void GettingHit()
    {
        hitStun--;
        if (hitStun <= 0) { EndState();/*healthManager.PoiseReset();*/ }
        curHitAnim += (targetHitAnim - curHitAnim) * .1f;//blends for 3D games
    }
    [Header("EnemyLogic")]
    public CharacterObject target;
    public float aggroRange = 30f, longAttackRange = 10f, shortAttackRange = 5f, attackCooldown = 180f;
    [SerializeField] private bool isNearPlayer, isLongRange, isShortRange;
    public int[] closeAttackState, rangedAttackState, desperationCAStates, desperationRAStates;

    [Tooltip("0 = MoveForward, 1 = MoveTowards, 2 = JumpAction")]
    public int enemyType;
    public int desperationTransitionState;

    [Space]
    [Header("Blocking States")]
    public bool canDefend = false;
    public bool IsDefendingInState()
    {
        for (int i = 0; i < defStates.Length; i++)
        {
            if (currentState==defStates[i])
            {
                return true;
            }
        }
        return false;
    }
    public int defStateIndex;
    public int[] defStates;

    public Vector2 FacingDir { get => facingDir; set => facingDir = value; }

    private void UpdateAI()
    {
        if (target == null)
        {
            FindTarget();
        }
        if (currentState == 0)//Neutral
        {
            if (isNearPlayer&&!isShortRange)
            {
                FaceTarget(target.transform.position);
                switch (enemyType)
                {
                    case 0:
                        FrontVelocity(moveSpeed);
                        break;
                    case 1:
                        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed);
                        break;
                    case 2:
                        StartState(rangedAttackState[0]);
                        break;
                }
            }
            if (isLongRange && rangedAttackState.Length > 0 && dashCooldown <= 0)
            {
                int randNum = UnityEngine.Random.Range(0, rangedAttackState.Length);
                StartState(rangedAttackState[randNum]);
            }
            if (isShortRange && closeAttackState.Length > 0 && dashCooldown <= 0)
            {
                int randNum = UnityEngine.Random.Range(0, closeAttackState.Length);
                StartState(closeAttackState[randNum]);
            }
        }
        if (currentState != 0)//Attack
        {
            dashCooldown = attackCooldown;
        }
    }
    //public void OnDesperation()
    //{
    //    closeAttackState = desperationCAStates;
    //    rangedAttackState = desperationRAStates;
    //    StartState(desperationTransitionState);
    //    attackCooldown *= 0.5f;
    //}
    public void OnDeath()
    {
        StartState(deathStateIndex);
        controlType = ControlType.DEAD;
        invulCooldown = 0f;
        spriteRend.color = Color.white;
        spriteRend.material = defaultMat;
        SetVelocity(Vector2.zero);

    }
    public void OnEnemySpawn()
    {
        controlType = ControlType.AI;
        StartState(0);
    }
    public void OnObjectSpawn()
    {
        controlType = ControlType.OBJECT;
        StartState(0);
    }
    public void OnBossSpawn()
    {
        controlType = ControlType.BOSS;
        StartState(0);
    }
    void FindTarget()
    {
        target = GameEngine.gameEngine.mainCharacter;
    }
    void FaceTarget(Vector3 tarPos)
    {
        Vector3 tarOffset = (tarPos - transform.position);
        direction = Mathf.Sign(tarOffset.x);
        transform.localScale = new Vector3(direction, 1f, 1f);
    }
    public void FaceDir(float dir)
    {
        direction = Mathf.Sign(dir);
        transform.localScale = new Vector3(direction, 1f, 1f);
    }
    public void FacePlayer()
    {
        FaceTarget(GameEngine.gameEngine.mainCharacter.transform.position);
    }
}
