using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;
//using UnityEditor.PackageManager.UI; causes error
using System;

public class NewNpcScr : MonoBehaviour
{
    #region Variables
    private bool isAlive,rayAim,rayLow,rayHigh,raySearch,raySearchLeft,raySearchRight,rayGround,navActive,animActive,rotateActive,controllerActive,
            rayJump,rayJumpForward,isFlying,isPathing,canSwap,isMoving,isAttached,nearPlayer,canAttack,canShoot,canMelee,canThrow,importantCharacter,
            dnaSet,canSwapWep,countMe,pathChecked,isGrounded,rayGroundHigh,rayIndoors,
            headIntact,armLeftIntact,armRightIntact,chestIntact,legLeftIntact,legRightIntact,friendlyFire,isCrouched,footsteps,
            inWater, isUnderwater, hungerActive, witnessed,canSeeEnemy,rayObstacle,isObstacle,calculatingObstacle,
            obstacleCalculationComplete,navResetActive,isIndoors;
    public bool isActive,inBattle,holdingFlag,isFollower,hasLeader,isLeader,hasSquad;
    [HideInInspector] public bool canRotate,canBleed,canMove,isTalking, hasRoutine,isNavPlan,navGoalMet,hasBodyParts,manualPathOverride;
    private int swapWep,tempWep,ranInt,attackChanceMax,greCount,grenadeChanceMax,nearEnemyCount,ammo,ammoMax,
    pathStep,nextPathStep,pathCount,healObjCount,bloodType,tempInt,swapInt,nearAllyCount,goreCount,searchChanceMax,
    greType,attackChanceGrenade,navScaleCount;
    [HideInInspector] public int killCount,dataPoints,navPlanStep,navPlanStepCount;
    public int curWep,reputation;
    
    private float maxHp,maxShields,navTimer,navMin,navMax,navRange,navPathRange,moveSpeed,totalPathDist,
    moveSpeedMax,ranFloat,rechargeTimer,recoveryTimer,rechargeRate,navResetTimer,
    shootDist,attackTimer,attackMin,attackMax,targetTimer,targetTimerLimit,navDistance,playerDistance,reloadTime,rotateSpeed,
    nearestEnemyDist,nearestAllyDist,nearestResourceDist,resourceDist,sightAngle,rayDistForward,rayDistDown,shootHeight,battleTimer,dmg,
    navMatrixTotal,fireSpeed,meleeDmg,climbSpeed,swapTimer,sightTimer,tempFloat,pathTimer,gravity,
    fireAimTimer,randomAimTimer,animationAimTimer,shootPosMod,meleeTimer,talkTimer,meleeDist,walkDist,pathWalkDist,navTargetObjDist,
    checkPathTimer,enemySearchTimer,navOverrideTimer,aimTargetOverrideTimer,forgiveTimer,animOverrideTimer,interactTimer,
    crouchHeight,regHeight,sumNavWeights,navWeightMod,animalTimer,expAmount,hunger,waterAir,
        shootPosModForward, footstepTimer,hitAngle,navGoalScore,navGoalLimit,impulseTimer,
        raySearchLeftVal,raySearchRightVal,jumpTimer,obstacleTimer,navScale,tempNavX,tempNavY,tempNavZ,moveHoldTimer;
    public float hp,shields;
    [HideInInspector] public float headHp,armLeftHp,armRightHp,legLeftHp,legRightHp,routineTimer,topNavControl,topNavGoal,topNavPlan,
    setNavGoalTimer,setNavPlanTimer,threatScore,curThreatScore,hitScore,finalNavTimer;
    private string navType,navAction,curArea,tempString,navControl,navGoal,navPlan;
    public string objName,objTeam,controlNavType;
    [HideInInspector] public string deathType,curRoutine,routineName,outfit;
    
    private GameObject controlObj,playerObj,curBul,curMelee,curGre,nearestEnemy,nearestAlly,navTargetObj,
    nearestResource,swapObj,tempDead,tempSwap,tempObj,nearestTarget, abilityObj,myTent;
    public GameObject deadObj,wepsObj,meleeObj,head,armLeft,armRight,legLeft,legRight,chest, recentEnemy,
    leaderObj,outfitHead,outfitArmLeft,outfitArmRight,outfitLegLeft,outfitLegRight,outfitChest,otherTalker,otherTalkerStarter,otherTalkerReceiver;
    private Vector3 originPos,navTarget,ranTarget,shootPos,nearestEnemyPos,lastEnemyPos,moveDir,aimTarget,
    ranRotateVal,pathNavTarget,previousPos,desiredMove,tempNavTarget,finalNavTarget,nearestResourcePos,
    relativePos,relativePosPlayer,aimTargetOverride,obstaclePos,tempNavPosition;
    public AudioSource sounds,soundsTalk,footstepSounds;
    public AudioClip fireSound;
    private Animator myAnimator;
    private Animation myAnimation;
    private NewPlayerScr playerScr;
    private NewControlScr controlScr;
    private BulScr bulScr;
    private grenadeScr greScr;
    private meleeScr melScr;
    private pickWepScr wepScr;
    private CharacterController controller;

    private CapsuleCollider capCol;
    private RaycastHit rayAimHit,rayHighHit,rayLowHit,rayGroundHit,rayJumpHit,rayJumpForwardHit,hitInfo,raySearchHit,raySearchLeftHit,raySearchRightHit,
    rayGroundHighHit,rayObstacleHit,rayIndoorsHit;
    private CollisionFlags colFlags;
    private MeshRenderer render;
    private SkinnedMeshRenderer skinRender;
    private Color genColor;
    private deadScr deathScr;
    private Quaternion targetRot;
    
    private GameObject[] enemies,allies,resources;
    [HideInInspector] public List<string> inventoryList = new List<string>();
    private List<Vector3> pathList = new List<Vector3>();
    //private List<Vector3> pathListPositions = new List<Vector3>();
    private List<string> navList = new List<string>();
    private List<string> navControlsList = new List<string>();
    private List<string> navGoalsList = new List<string>();
    private List<string> navPlansList = new List<string>();

    private List<GameObject> recentAllies = new List<GameObject>();
    private List<GameObject> recentEnemies = new List<GameObject>();

    private List<string> navPlanActions = new List<string>();
    private List<MeshRenderer> meshRens = new List<MeshRenderer>();
    private List<SkinnedMeshRenderer> skinnedMeshRens = new List<SkinnedMeshRenderer>();
    [HideInInspector] public List<string> navActions = new List<string>();

    private List<string> enemyTags = new List<string>();

    private List<string> allyTags = new List<string>();

    public List<AudioClip> meleeSounds = new List<AudioClip>();
    public List<AudioClip> hitSounds = new List<AudioClip>();
    public List<AudioClip> flankSounds = new List<AudioClip>();
    public List<AudioClip> flushoutSounds = new List<AudioClip>();
    public List<AudioClip> focusfireSounds = new List<AudioClip>();
    public List<AudioClip> groupupSounds = new List<AudioClip>();
    public List<AudioClip> angerSounds = new List<AudioClip>();
    public List<AudioClip> tauntSounds = new List<AudioClip>();
    public List<AudioClip> grenadeSounds = new List<AudioClip>();
    public List<AudioClip> sightSounds = new List<AudioClip>();
    public List<AudioClip> lostSightSounds = new List<AudioClip>();
    public List<AudioClip> helpSounds = new List<AudioClip>();
    public List<AudioClip> killSounds = new List<AudioClip>();
    public List<AudioClip> allyKillSounds = new List<AudioClip>();
    public List<AudioClip> followSounds = new List<AudioClip>();
    public List<AudioClip> retreatSounds = new List<AudioClip>();
    public List<AudioClip> casualtySounds = new List<AudioClip>();

    public List<AudioClip> stepSounds = new List<AudioClip>();

    public List<GameObject> squadObjs = new List<GameObject>();

    List<GameObject> finalPath=new List<GameObject>();
    List<GameObject> openNodes=new List<GameObject>();
    List<GameObject> curNodes=new List<GameObject>();

    private interactiveScr intScr;
    private NewConvoScr myConvoScr;

    [HideInInspector] public Dictionary<string, float> navDef = new Dictionary<string, float>();
    public Dictionary<string, float> navControls = new Dictionary<string, float>();
    public Dictionary<string, float> navGoals = new Dictionary<string, float>();
    
    public Dictionary<string, float> navPlans = new Dictionary<string, float>();
    public Dictionary<GameObject, float> recentEnemyScores = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> recentAllyScores = new Dictionary<GameObject, float>();

    Collider[] hitColliders;
    
    public List<GameObject> nearObjs=new List<GameObject>();

    //New navigation pathfinding system vars
    public List<Vector3> navPositions = new List<Vector3>();

    //Talk collider
    public SphereCollider talkCollider;

    #endregion

    void Awake()//Initializes object and ensures components are set.
    {
        navTarget=transform.position;
        finalNavTarget=transform.position;
        tempNavTarget=transform.position;
        pathNavTarget=transform.position;
        canRotate=true;
        rotateActive=true;
        navActive=true;
        moveDir = Vector3.zero;
        isPathing=false;
        friendlyFire=false;
        hunger = 100f;
        hungerActive = false;
        waterAir = 10f;
        meleeDist = 1f;
        navGoalLimit=100f;
        canSeeEnemy=false;
        curWep=100;
        if (GameObject.FindGameObjectWithTag("GameController") != null)
        {
            controlObj = GameObject.FindGameObjectWithTag("GameController");
            controlScr = controlObj.gameObject.GetComponent<NewControlScr>();
        }
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            playerScr = playerObj.gameObject.GetComponent<NewPlayerScr>();
        }
        this.gameObject.name = this.gameObject.name.Replace("(Clone)", "").Trim();
        if(string.IsNullOrEmpty(objName)){
            objName=this.gameObject.name;
        }
        if(GetComponent<AudioSource>()!=null){
            sounds = GetComponent<AudioSource>();
        }else{
            sounds = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        }
        if(soundsTalk==null){
            soundsTalk = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            soundsTalk.spatialBlend=1;
            soundsTalk.maxDistance=50f;
        }
        if (GetComponent<Animator>() != null)
        {
            myAnimator = GetComponent<Animator>();
        }
        if (gameObject.GetComponent<Animation>() != null)
        {
            myAnimation = GetComponent<Animation>();
        }
        originPos=transform.position;
        if (GetComponent<CharacterController>() != null)
        {
            controller = GetComponent<CharacterController>();
            controllerActive=true;
            regHeight=controller.height;
            crouchHeight=regHeight*0.4f;
            if (GetComponent<CapsuleCollider>() != null){
                capCol=GetComponent<CapsuleCollider>();
            }
        }else{
            controllerActive=false;
        }
        if (GetComponent<inventoryScr>() != null)
        {
            inventoryList = GetComponent<inventoryScr>().itemList;
        }
        if(GetComponent<NewConvoScr>()!=null){
            myConvoScr = GetComponent<NewConvoScr>();
        }
        if(wepsObj==null){
            if(controlScr.RecursiveFindChild(transform,"weapons")!=null){
                wepsObj=controlScr.RecursiveFindChild(transform,"weapons").gameObject;
            }
        }
        gravity=Physics.gravity.y;
        if(stepSounds!=null){
            if(stepSounds.Count>0){
                footsteps=true;
                footstepSounds=this.gameObject.AddComponent<AudioSource>();
                footstepSounds.spatialBlend=1f;
                footstepSounds.maxDistance=10f;
            }
        }
        talkTimer=UnityEngine.Random.Range(10f,30f);
    }

    void Start(){
        SetNavLists();
        SetObjectDefaults();
        if(meleeObj==null){
            meleeObj=controlScr.meleeObj;
        }
        /*
        if(Terrain.activeTerrain!=null){
            if(transform.position.y<Terrain.activeTerrain.SampleHeight(transform.position)){
                transform.position=new Vector3(transform.position.x,
                Terrain.activeTerrain.SampleHeight(transform.position)+2f,
                transform.position.z);
            }
        }
        */
        if(controlScr.navNodes!=null){
            openNodes=controlScr.navNodes;
        }
        SetNavControl();
    }

    void SetObjectDefaults(){//Sets defaults and changes parameters based on npc type
        rayDistForward=5f;
        pathWalkDist=walkDist;
        rechargeRate=10f;
        isIndoors=false;
        hp=100;
        shields=0;
        shootDist=75f;
        canShoot=true;
        navType="default";
        lastEnemyPos=transform.position;
        attackChanceGrenade=10;
        impulseTimer=0f;
        manualPathOverride=false;
        if(GetComponent<CharacterController>()!=null){

        }
        if(string.IsNullOrEmpty(controlNavType)){
            controlNavType="none";
        }
        rotateSpeed=5f;
        attackChanceMax = 5;
        searchChanceMax=5;
        canMove=true;
        switch(controlScr.gameMode){
            default:
                moveSpeed=10f;
                break;
            case "Deathmatch":
            case "Capture the flag":
                moveSpeed=controlScr.multiplayerMoveSpeed;
                navTargetObj=controlScr.flagObj;
                Alert();
                break;
        }
        meleeDmg=25f;
        climbSpeed=5f;
        canSwapWep=true;
        canBleed=true;
        bloodType=0;
        countMe=true;
        swapInt=100;
        isFlying=false;
        goreCount=0;
        shootPosMod=2f;
        shootPosModForward = 1f;
        navMin=4f;
        navMax=navMin*3f;
        canAttack=true;
        canMelee=true;
        SetObjName();
        SetNavType();
        SetBodyParts();
        SetUpgrades();
        walkDist=rayDistForward;
        pathWalkDist=walkDist;
        navRange=1f+(3f*moveSpeed);
        rayDistDown=climbSpeed*2f;
        maxHp=hp;
        maxShields=shields;
        isAlive=true;
        objTeam=gameObject.tag;
        if(gameObject.tag=="npc"){
            importantCharacter=true;
            controlNavType="hold";
            //Test set outfit
            SetOutfit();
            //Set NPC teams
            switch(objName){
                default:
                    break;
                case "LyraStarling":
                    objTeam="coalition";
                    break;
                case "OrionPaxton":
                    objTeam="collective";
                    break;
                case "ZephyrDrakon":
                    objTeam="dominion";
                    break;
                case "JhetaZen":
                    objTeam="coalition";
                    break;
                case "LamisaTalil":
                    objTeam="coalition";
                    break;
                case "DiogenHan":
                    objTeam="coalition";
                    break;
                case "AccessPanel":
                case "AccessPoint":
                    objTeam="unf";
                    canMove=false;
                    break;
                case "AmalMoore":
                    objTeam="coalition";
                    //SetOutfit("","manhair1");
                    break;
                case "SaraTrenn":
                    objTeam="coalition";
                    break;
                case "BronsonKeeling":
                    objTeam="coalition";
                    break;
                case "JahRook":
                    objTeam="coalition";
                    break;
                case "DelarusGuard":
                    objTeam="coalition";
                    //SetOutfit("","manhair1");
                    break;
                case "LamontDerrel":
                    objTeam="coalition";
                    break;
                case "CharrMeek":
                    objTeam="coalition";
                    break;
                case "GupTor":
                    objTeam="collective";
                    break;
                case "NahliaJericho":
                    objTeam="collective";
                    break;
                case "TheromScandos":
                    objTeam="collective";
                    break;
                case "BanditLeader":
                    objTeam="bandit";
                    break;
            }
        }else{
            importantCharacter=false;
        }
        SetRep();
        SetWeapon(curWep);
    }

    void SetNavLists(){
        navControlsList.Add("idle");
        navControlsList.Add("patrol");
        navControlsList.Add("patrolleader");
        navControlsList.Add("patrolfollower");
        navControlsList.Add("routine");
        navControlsList.Add("combat");
        navControlsList.Add("combatleader");
        navControlsList.Add("combatfollower");

        navGoalsList.Add("survive");
        navGoalsList.Add("cleararea");
        navGoalsList.Add("group");
        navGoalsList.Add("routine");
        navGoalsList.Add("hide");
        navGoalsList.Add("hunt");
        navGoalsList.Add("findweaponranged");
        navGoalsList.Add("findweaponclose");
        navGoalsList.Add("findenemy");

        navPlansList.Add("flankleft");
        navPlansList.Add("flankright");
        navPlansList.Add("followpath");
        navPlansList.Add("leadgroup");
        navPlansList.Add("followgroup");
        navPlansList.Add("groupup");
        navPlansList.Add("evade");
        navPlansList.Add("pursue");
        navPlansList.Add("search");
        navPlansList.Add("routine");
        navPlansList.Add("flushout");
        navPlansList.Add("patrol");
        navPlansList.Add("reactdamage");
        navPlansList.Add("blocked");
        navPlansList.Add("door");
        navPlansList.Add("reposition");
        navPlansList.Add("findweaponranged");
        navPlansList.Add("findweaponclose");
        navPlansList.Add("findpath");

        foreach(string nvc in navControlsList){
            navControls.Add(nvc,0f);
        }
        foreach(string nvg in navGoalsList){
            navGoals.Add(nvg,0f);
        }
        foreach(string nvp in navPlansList){
            navPlans.Add(nvp,0f);
        }
    }

    public IEnumerator LoadUpdateSettings(int cw,string cn,int kc){
        yield return new WaitForSeconds(0.1f); 
        curWep=cw;
        SetWeapon(cw);
        controlNavType=cn;
        if(controlNavType!="none"){
            ChangeNavSettings(cn);
        }
        killCount=kc;
        if(talkCollider!=null){
            switch(controlNavType){
                default:
                    switch(objTeam){
                        default:
                            talkCollider.radius=10;
                            break;
                        case "AmalMoore":
                            talkCollider.radius=10;
                            break;
                    }
                    break;
                case "hold":
                case "none":
                    break;
            }
        }
    }

    void SetZeronium(){
        switch(objTeam){
            default:
                break;
            case "dominion":
                controlScr.dominionZeronium-=10;
                break;
            case "collective":
                controlScr.collectiveZeronium-=10;
                break;
            case "coalition":
                controlScr.nfcZeronium-=10;
                break;
            case "zerran":
                controlScr.zerranZeronium-=10;
                break;
            case "alharean":
                controlScr.alhareanZeronium-=10;
                break;
        }
    }

    void SetUpgrades(){
        switch(objTeam){
            default:
                break;
            case "alharean":
                maxHp=hp+=(0.001f*controlScr.alhareanZeronium);
                break;
            case "dominion":
                maxHp=hp+=(0.001f*controlScr.dominionZeronium);
                break;
            case "collective":
                maxHp=hp+=(0.001f*controlScr.collectiveZeronium);
                break;
            case "coalition":
                maxHp=hp+=(0.001f*controlScr.nfcZeronium);
                break;
            case "zerran":
                maxHp=hp+=(0.001f*controlScr.zerranZeronium);
                break;
        }
    }

    void SetObjName(){
        expAmount=10f;
        isUnderwater = false;
        hasRoutine = false;
        switch (objName){
            default:
                navType="none";
                break;
            case "bandit":
                navType="lead";
                curWep=UnityEngine.Random.Range(0,1);
                break;
            case "coalitionCivilian":
            case "rebelCivilian":
                hp=50f;
                curWep=100;
                navMin=30f;
                navType="hide";
                if (!dnaSet){
                    dnaSet = true;
                    Generate("random",0.5f);
                }
                expAmount/=2f;
                moveSpeed*=0.25f;
                hasRoutine = true;
                routineName = "civilian";
                hungerActive = true;
                //canMelee=false;
                break;
            case "animal1":
                hp=60f;
                curWep=100;
                navMin=10f;
                navType="hide";
                shootPosMod=1.5f;
                expAmount/=2f;
                canSwapWep=false;
                break;
            case "animal2":
                hp = 150f;
                curWep = 100;
                navMin=12f;
                navType = "hide";
                shootPosMod = 2.5f;
                expAmount *= 2f;
                isUnderwater = true;
                rayDistForward *= 2f;
                canSwapWep=false;
                break;
            case "animal3":
                hp = 30f;
                curWep = 100;
                navMin= 0.5f;
                navType = "flycover";
                shootPosMod = 1f;
                expAmount /= 2f;
                isFlying = true;
                moveSpeed *= 2f;
                climbSpeed*= 3f;
                navRange *= 30f;
                canSwapWep=false;
                break;
            case "refugeeCivilian":
                hp=40f;
                curWep=100;
                navMin=30f;
                navType="hide";
                if (!dnaSet){
                    dnaSet = true;
                    Generate("random",0.5f);
                }
                expAmount/=2f;
                moveSpeed*=0.25f;
                hasRoutine = true;
                routineName = "refugee";
                hungerActive = true;
                canMelee=false;
                break;
            case "dominionCivilian":
                hp=50f;
                curWep=100;
                navMin=30f;
                navType="hide";
                moveSpeed*=0.25f;
                if (!dnaSet){
                    dnaSet = true;
                    Generate("random",0.5f);
                }
                expAmount/=2f;
                hasRoutine = true;
                routineName = "civilian";
                hungerActive = true;
                break;
            case "coalitionSoldierMultiplayer":
            case "dominionSoldierMultiplayer":
                shields=100f;
                curWep=0;
                navMin=1f;
                greCount=1;
                break;
            case "coalitionSoldier":
                curWep=UnityEngine.Random.Range(0,1);
                moveSpeed*=1.5f;
                greCount=1;
                break;
            case "coalitionTank":
            case "dominionTank":
                navType="lead";
                hp=600f;
                shields=200f;
                curWep=101;
                navMin=6f;
                moveSpeed*=0.75f;
                canSwapWep=false;
                canMelee=false;
                canBleed=false;
                expAmount*=5f;
                shootPosMod=4f;
                shootPosModForward = 3f;
                shootDist*=2f;
                rotateSpeed/=5f;
                break;
            case "coalitionHovercraft":
            case "dominionHovercraft":
                hp=250f;
                shields=100f;
                isFlying=true;
                navType="fly";
                curWep=102;
                navMin=3f;
                canMelee=false;
                canBleed=false;
                climbSpeed*=5f;
                moveSpeed*=1.5f;
                rayDistForward*=2f;
                shootPosMod=(-2f);
                expAmount*=5f;
                shootDist*=3f;
                break;
            case "dominionSoldier":
                hp=150f;
                navType="follow";
                ranInt=UnityEngine.Random.Range(0,5);
                switch(ranInt){
                    default:
                    case 0:
                    case 1:
                    case 2:
                        curWep=0;//Rifle
                        break;
                    case 3:
                    case 4:
                        curWep=1;//Pistol
                        break;
                    case 5:
                        curWep=5;//Sniper
                        break;
                }
                greCount=1;
                isFollower=true;
                break;
            case "dominionEnforcer":
                hp=150f;
                shields=100f;
                navType="lead";
                curWep=2;//Shotgun
                greCount=2;
                healObjCount=1;
                moveSpeed*=1.75f;
                navMin=3f;
                expAmount*=2.5f;
                isLeader=true;
                break;
            case "zerranSoldier":
                hp=50f;
                shields=100f;
                navType="follow";
                ranInt=UnityEngine.Random.Range(0,5);
                switch(ranInt){
                    default:
                    case 0:
                    case 1:
                    case 2:
                        curWep=4;//Lancer
                        break;
                    case 3:
                    case 4:
                        curWep=7;//Razor
                        break;
                    case 5:
                        curWep=6;//Rail
                        break;
                }
                bloodType=1;
                greCount=1;
                greType=1;
                isFollower=true;
                break;
            case "zerranCharger":
                hp=150f;
                shields=150f;
                navType="lead";
                curWep=3;//Void
                navMin=3f;
                moveSpeed*=1.9f;
                bloodType=1;
                greCount=2;
                greType=1;
                expAmount*=2.5f;
                isLeader=true;
                break;
            case "zerranHunter":
                hp=200f;
                shields=150f;
                navType="chase";
                curWep=103;//Beam
                climbSpeed*=4f;
                moveSpeed*=4f;
                navMin=1f;
                canBleed=false;
                meleeDist*=5f;
                shootPosMod=4f;
                expAmount*=5f;
                rayDistForward*=3f;
                isLeader=true;
                break;
            case "alhareanSoldier":
                hp=250f;
                shields=450f;
                navType="normal";
                curWep=103;//Beam
                climbSpeed*=5f;
                moveSpeed*=1.25f;
                navMin=4f;
                canBleed=false;
                meleeDist*=5f;
                shootPosMod=4f;
                expAmount*=12f;
                rayDistForward*=3f;
                rotateSpeed*=1.25f;
                isLeader=true;
                break;
            case "zerranCrawler":
                hp=2500f;
                shields=1000f;
                navType="hide";
                moveSpeed=0.4f;
                curWep=103;//Beam
                navMin=15f;
                canBleed=false;
                expAmount*=100f;
                isLeader=true;
                break;
            case "zerranProbe":
                curWep=103;//Beam
                isFlying=true;
                navType="fly";
                canMelee=false;
                navMin=3f;
                canBleed=false;
                climbSpeed*=5f;
                moveSpeed*=1.1f;
                rayDistForward*=4f;
                shootPosMod=(-2f);
                expAmount*=5f;
                break;
            case "alhareanCruiser":
                curWep=103;//Beam
                hp=2000f;
                shields=1000f;
                isFlying=true;
                navType="fly";
                canMelee=false;
                navMin=10f;
                canBleed=false;
                climbSpeed*=4f;
                moveSpeed*=0.5f;
                rayDistForward*=4f;
                shootPosMod=(-2f);
                expAmount*=20f;
                break;
            case "zerranCarrier":
                hp=250f;
                shields=250f;
                curWep=103;//Beam
                canMelee=false;
                navType="hide";
                canBleed=false;
                moveSpeed*=3f;
                climbSpeed*=5f;
                shootPosMod=25f;
                expAmount*=10f;
                isLeader=true;
                break;
            case "rebelSoldier":
                navType="follow";
                moveSpeed*=1.2f;
                curWep=0;
                break;
            case "unfSoldier":
                shields=250f;
                curWep=0;
                navType="lead";
                moveSpeed*=1.4f;
                break;
            case "unfTurret":
            case "coalitionTurret":
            case "dominionTurret":
            case "collectiveTurret":
                shields=100f;
                canBleed=false;
                canMove=false;
                controlNavType="hold";
                curWep=102;
                navType="none";
                moveSpeed=0;
                navMin=30f;
                break;
            case "DelarusComputer":
                canBleed=false;
                canRotate=false;
                navType="none";
                hp=99999f;
                shields=0;
                //Easter egg
                expAmount*=1000f;
                break;
        }
        meleeDist = rayDistForward / 2f;
        threatScore=expAmount;
        navMax =navMin*=3f;
        if (GetComponent<CharacterController>() == null){
            controller=gameObject.AddComponent(typeof(CharacterController)) as CharacterController;
        }
    }

    void SetNavType(string navOverride=""){
        if(!string.IsNullOrEmpty(navOverride)){
            navType=navOverride;
        }else{
            switch(controlNavType){
                default:
                    navType=controlNavType;
                    break;
                case "none":
                case "hold":
                    break;
            }
        }
        switch(controlScr.gameMode){
            default:
            case "campaign":
                switch(navType){
                    default:
                    case "normal":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 10f, 40f, 20f, 30f,0f});
                        break;
                    case "tagalong":
                        NavMatrix("set", new string[] { "tagalong", "random"},
                        new float[] {80f,20f});
                        break;
                    case "chase":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 0f, 70f, 10f, 20f,0f});
                        break;
                    case "lead":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 10f, 50f, 10f, 30f,0f});
                        break;
                    case "follow":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 20f, 20f, 40f, 20f,0f});
                        break;
                    case "fly":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 0f, 20f, 30f, 30f,20f});
                        break;
                    case "flycover":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random", "origin" },
                        new float[] { 30f, 0f, 20f, 40f, 10f });
                        break;
                    case "hide":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 30f, 0f, 5f, 5f,60f});
                        break;
                    case "routine":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin", "target","special"},
                        new float[] { 0f, 0f, 0f, 25f,25f ,0f,50f});
                        break;
                    case "target":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin", "target","special"},
                        new float[] { 0f, 0f, 0f, 40f,0f ,60f,0f});
                        break;
                    case "none":
                        NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin"},
                        new float[] { 0f, 0f, 0f, 0f,100f});
                        navActive=false;
                        navMin=30f;
                        break;
                }
                break;
            case "Deathmatch":
                NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin", "target","special","weapon"},
                        new float[] { 5f, 45f, 20f, 20f,0f ,0f,0f,10f});
                navType="normal";
                break;
            case "Capture the flag":
                NavMatrix("set",
                new string[] { "flag","follow", "help", "random","cover","weapon" },
                new float[] { 45f,10f, 10f, 20f,5f,10f });
                NavMatrix("set", new string[] { "cover", "follow", "help", "random","origin", "target","special","flag"},
                        new float[] { 10f, 40f, 20f, 30f,0f ,0f,0f});
                navType="normal";
                break; 
        }
    }

    public void ChangeRep(int cr,bool setRep=true){
        reputation=cr;
        if(setRep){
            switch(objTeam){
                default:
                break;
                case "dominion":
                    controlScr.repDominion=cr;
                    break;
                case "zerran":
                    controlScr.repZerran=cr;
                    break;
                case "coalition":
                    controlScr.repCoalition=cr;
                    break;
                case "collective":
                    controlScr.repCollective=cr;
                    break;
                case "unf":
                    controlScr.repUnf=cr;
                    break;
                case "bandit":
                    controlScr.repBandits=cr;
                    break;
                case "animal":
                    controlScr.repAnimals=cr;
                    break;
                case "alharean":
                    controlScr.repAlharean=cr;
                    break;  
            }
            for (int i = 0; i < allyTags.Count; i++){
                allies = GameObject.FindGameObjectsWithTag(allyTags[i]);
                foreach (GameObject ally in allies){
                    if (ally.GetComponent<NewNpcScr>() != null){
                        if (Vector3.Distance(transform.position, ally.transform.position) < navRange){
                            ally.GetComponent<NewNpcScr>().Talk("allykill");
                        }
                        ally.GetComponent<NewNpcScr>().SetRep();
                    }
                }
            }
            allies = GameObject.FindGameObjectsWithTag("npc");
            foreach (GameObject ally in allies){
                if (ally.GetComponent<NewNpcScr>() != null){
                    if(ally.GetComponent<NewNpcScr>().objTeam==objTeam){
                        ally.GetComponent<NewNpcScr>().SetRep(true,cr);
                    }
                }
            }
            SetRep();
        }else{
            SetRep(true,cr);
        }
        
    }

    public void PlayAnim(string pa, bool overrider=false,float overtimer=0f){
        if(myAnimation!=null){
            if(!myAnimation.IsPlaying(pa)){
                if(animOverrideTimer<=0){
                    if(overrider){
                        if(shields<=0f){
                            animOverrideTimer+=overtimer;
                            moveHoldTimer+=UnityEngine.Random.Range(0f,hp-(hp-overtimer/hp));
                            attackTimer+=UnityEngine.Random.Range(0f,hp-(hp-overtimer/hp));
                        }
                    }
                    if(myAnimation.GetClip(pa)!=null){
                        myAnimation.Play(pa);
                    }
                }
            }
        }
    }

    void SetRep(bool ovr=false,int repover=0){
        enemyTags.Clear();
        allyTags.Clear();
        allyTags.Add(objTeam);
        reputation=1;
        switch(objTeam){
            default:
            break;
            case "alharean":
                reputation=controlScr.repAlharean;
                enemyTags.Add("dominion");
                enemyTags.Add("zerran");
                enemyTags.Add("unf");
                enemyTags.Add("coalition");
                enemyTags.Add("collective");
                enemyTags.Add("bandit");
                enemyTags.Add("animal");
                break;
            case "dominion":
                reputation=controlScr.repDominion;
                enemyTags.Add("zerran");
                enemyTags.Add("unf");
                enemyTags.Add("coalition");
                enemyTags.Add("collective");
                enemyTags.Add("bandit");
                enemyTags.Add("alharean");
                allyTags.Add("animal");
                break;
            case "zerran":
                reputation=controlScr.repZerran;
                enemyTags.Add("dominion");
                enemyTags.Add("unf");
                enemyTags.Add("coalition");
                enemyTags.Add("collective");
                enemyTags.Add("bandit");
                allyTags.Add("animal");
                enemyTags.Add("alharean");
                break;
            case "unf":
                reputation=controlScr.repUnf;
                enemyTags.Add("zerran");
                enemyTags.Add("dominion");
                allyTags.Add("coalition");//Allied to coalition
                enemyTags.Add("collective");
                enemyTags.Add("bandit");
                allyTags.Add("animal");
                enemyTags.Add("alharean");
                break;
            case "collective":
                reputation=controlScr.repCollective;
                enemyTags.Add("zerran");
                enemyTags.Add("dominion");
                enemyTags.Add("unf");
                enemyTags.Add("coalition");
                enemyTags.Add("bandit");
                allyTags.Add("animal");
                enemyTags.Add("alharean");
                break;
            case "coalition":
                reputation=controlScr.repCoalition;
                enemyTags.Add("zerran");
                enemyTags.Add("dominion");
                allyTags.Add("unf");//Allied to unf
                enemyTags.Add("collective");
                enemyTags.Add("bandit");
                allyTags.Add("animal");
                enemyTags.Add("alharean");
                break;
            case "bandit":
                reputation=controlScr.repBandits;
                enemyTags.Add("zerran");
                enemyTags.Add("dominion");
                enemyTags.Add("unf");
                enemyTags.Add("collective");
                allyTags.Add("bandit");
                allyTags.Add("animal");
                enemyTags.Add("alharean");
                break;
            case "animal":
                reputation=controlScr.repAnimals;
                allyTags.Add("zerran");
                enemyTags.Add("dominion");
                enemyTags.Add("unf");
                enemyTags.Add("collective");
                enemyTags.Add("bandit");
                allyTags.Add("animal");
                enemyTags.Add("alharean");
                break;
        }
        switch(controlNavType){
            default:
                break;
            case "ally":
                reputation=1;
                break;
            case "enemy":
                reputation=0;
                break;
        }
        if(ovr){
            reputation=repover;
        }
        if(reputation<=0){
            enemyTags.Add("Player");
        }else{
            allyTags.Add("Player");
        }
        controlScr.UpdateReputation();
    }

    void Kill(){
        switch(objTeam){
            default:
            break;
            case "dominion":
                if(controlScr.gameMode=="Deathmatch"){
                    controlScr.scoreDominion+=1;
                }
            break;
            case "zerran":
            break;
            case "coalition":
                if(controlScr.gameMode=="Deathmatch"){
                    controlScr.scoreCoalition+=1;
                }
                break;
            case "collective":
            break;
            case "unf":
            break;
            case "bandit":
            break;
            case "alharean":
                break;
        }
        killCount+=1;
        TalkSounds("killSounds");
        if(nearestAlly!=null){
            if(nearestAlly.GetComponent<NewNpcScr>()!=null){
                nearestAlly.GetComponent<NewNpcScr>().AllyKill();
            }
            if(nearestAllyDist<50f){
                TalkSounds("allyKillSounds");
            }
        }
    }


    public void HearSounds(string typ,int rng=1){
        if(soundsTalk!=null){
            ranInt=UnityEngine.Random.Range(0,rng);
            if(talkTimer<=0){
                if(ranInt==0){
                    switch (typ){
                        default:
                            TalkSounds(typ);
                            break;
                        case "flankSounds":
                            TalkSounds("followSounds");
                            break;
                        case "casualtySounds":
                            TalkSounds("helpSounds");
                            break;
                        case "killSounds":
                            TalkSounds("followSounds");
                            break;
                        case "grenadeSounds":
                            TalkSounds("helpSounds");
                            break;
                        case "sightSounds":
                            TalkSounds("tauntSounds");
                            break;
                        case "retreatSounds":
                            TalkSounds("helpSounds");
                            break;
                        case "allyKillSounds":
                            TalkSounds("tauntSounds");
                            break;
                        case "helpSounds":
                            TalkSounds("lostSightSounds");
                            break;
                        case "followSounds":
                            TalkSounds("tauntSounds");
                            break;
                        case "lostSightSounds":
                            TalkSounds("retreatSounds");
                            break;
                            
                    }
                }
            }
        }
    }
    public void TalkSounds(string typ,int rng=4){
        if(soundsTalk!=null){
            ranInt=UnityEngine.Random.Range(0,rng);
            if(talkTimer<=0){
                if(ranInt==0){
                    if(nearAllyCount>0){
                        if(nearestAlly!=null){
                            if(nearestAlly.GetComponent<NewNpcScr>()!=null){
                                nearestAlly.GetComponent<NewNpcScr>().HearSounds(typ);
                            }
                        }
                    }
                    switch (typ){
                        default:
                            if(tauntSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,tauntSounds.Count-1);
                                soundsTalk.clip=tauntSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "casualtySounds":
                            if(casualtySounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,casualtySounds.Count-1);
                                soundsTalk.clip=casualtySounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "flankSounds":
                            if(flankSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,flankSounds.Count-1);
                                soundsTalk.clip=flankSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "flankleft":
                            if(flankSounds.Count>1){
                                soundsTalk.clip=flankSounds[1];
                                soundsTalk.Play();
                            }else{
                                if(flankSounds.Count>0){
                                    soundsTalk.clip=flankSounds[0];
                                    soundsTalk.Play();
                                }
                            }
                            break;
                        case "flankright":
                            if(flankSounds.Count>2){
                                soundsTalk.clip=flankSounds[2];
                                soundsTalk.Play();
                            }else{
                                if(flankSounds.Count>0){
                                    soundsTalk.clip=flankSounds[0];
                                    soundsTalk.Play();
                                }
                            }
                            break;
                        case "hitSounds":
                            if(hitSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,hitSounds.Count-1);
                                soundsTalk.clip=hitSounds[ranInt];
                                soundsTalk.Play();
                            }else{
                                ranInt=UnityEngine.Random.Range(0,controlScr.hitSounds.Length-1);
                                soundsTalk.clip=controlScr.hitSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "shieldHitSounds":
                            tempInt = UnityEngine.Random.Range(0, controlScr.shieldHitSounds.Length - 1);
                            sounds.clip = controlScr.shieldHitSounds[tempInt];
                            sounds.Play();
                            break;
                        case "killSounds":
                            if(killSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,killSounds.Count-1);
                                soundsTalk.clip=killSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "meleeSounds":
                            if(meleeSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,meleeSounds.Count-1);
                                soundsTalk.clip=meleeSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "tauntSounds":
                            if(tauntSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,tauntSounds.Count-1);
                                soundsTalk.clip=tauntSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "grenadeSounds":
                            if(grenadeSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,grenadeSounds.Count-1);
                                soundsTalk.clip=grenadeSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "sightSounds":
                            if(sightSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,sightSounds.Count-1);
                                soundsTalk.clip=sightSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "lostSightSounds":
                            if(lostSightSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,lostSightSounds.Count-1);
                                soundsTalk.clip=lostSightSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "helpSounds":
                            if(helpSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,helpSounds.Count-1);
                                soundsTalk.clip=helpSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "allyKillSounds":
                            if(allyKillSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,allyKillSounds.Count-1);
                                soundsTalk.clip=allyKillSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "followSounds":
                            if(followSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,followSounds.Count-1);
                                soundsTalk.clip=followSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                        case "retreatSounds":
                            if(retreatSounds.Count>0){
                                ranInt=UnityEngine.Random.Range(0,retreatSounds.Count-1);
                                soundsTalk.clip=retreatSounds[ranInt];
                                soundsTalk.Play();
                            }
                            break;
                    }
                }  
                talkTimer=UnityEngine.Random.Range(navMin,navMax);
            }
        }
    }

    void Update(){
        if(controlScr.isActive){
            if((isAlive)&&(isActive)){
                if (hp <= 0){
                    Death();
                }
                if(obstacleTimer>0f){
                    obstacleTimer-=Time.deltaTime;
                }
                if(talkTimer>0){
                    talkTimer-=Time.deltaTime;
                }
                if(moveHoldTimer>0){
                    moveHoldTimer-=Time.deltaTime;
                }
                if(!isMoving){
                    if (randomAimTimer>0){
                        randomAimTimer-=Time.deltaTime;
                    }else{
                        ChangeRotateVal();
                    }
                }
                if(nearPlayer){
                    if((!isTalking)&&(myConvoScr==null)){
                        if(Input.GetKey(KeyCode.E)){
                            Talk("banter");
                        }
                    }
                    
                }
                if(fireAimTimer>0){
                    fireAimTimer-=Time.deltaTime;
                }
                if(navResetActive){
                    if(navResetTimer>0){
                        navResetTimer-=Time.deltaTime;
                    }else{
                        navResetActive=false;
                        navActive=false;
                    }
                }
                if (hasRoutine)
                {
                    if (routineTimer > 0)
                    {
                        routineTimer -= Time.deltaTime;
                    }
                    else
                    {
                        ChangeRoutine();
                    }
                }
                if (hungerActive)
                {
                    if (hunger > 0f)
                    {
                        hunger -= Time.deltaTime/10f;
                        if (hunger < 25f)
                        {
                            ChangeRoutine("food");
                        }
                    }
                    else
                    {
                        Starving();
                    }
                }
                if (inWater)
                {
                    if(objTeam!="zerran"){
                        if (!isUnderwater)
                        {
                            if (waterAir > 0f)
                            {
                                waterAir -= Time.deltaTime;
                            }
                            else
                            {
                                hp -= 1f;
                            }
                        }
                    }
                    
                }
                if(enemySearchTimer>0){
                    enemySearchTimer-=Time.deltaTime;
                }
                if(animOverrideTimer>0){
                    animOverrideTimer-=Time.deltaTime;
                }
                if(friendlyFire){
                    if(forgiveTimer>0){
                        forgiveTimer-=Time.deltaTime;
                    }else{
                        ChangeRep(1,false);
                        friendlyFire=false;
                    }
                }
                if(interactTimer>0){
                    interactTimer-=Time.deltaTime;
                }
                if(objTeam=="animal"){
                    if(animalTimer>=0){
                        animalTimer-=Time.deltaTime;
                    }
                }
                if(aimTargetOverrideTimer>0){
                    aimTargetOverrideTimer-=Time.deltaTime;
                }
                if(navOverrideTimer>0){
                    navOverrideTimer-=Time.deltaTime;
                }
                if(impulseTimer>0){
                    impulseTimer-=Time.deltaTime;
                }
                if(pathChecked){
                    if(checkPathTimer>0f){
                        checkPathTimer-=Time.deltaTime;
                    }else{
                        pathChecked=false;
                    }
                }
                if(navActive){
                    if(isPathing){
                        if(pathTimer>0f){
                            pathTimer-=Time.deltaTime;
                        }else{
                            isPathing=false;
                        }
                    }
                    if(navTimer>0){
                        navTimer-=Time.deltaTime;
                    }else{
                        switch(controlNavType){
                            default:
                                //another problem here 04152024
                                if(isPathing){
                                    if(!pathChecked){
                                        //COMEBACK
                                        CheckPath();
                                    }
                                }else{
                                    Navigate();
                                }
                                break;
                            case "hold":

                                break;
                            case "tagalong":
                                //problem was here
                                Navigate();
                                break;
                            case "none":
                                if((isObstacle)||(navOverrideTimer>0)){
                                    Navigate("reset");
                                    Navigate("plan");
                                }else{
                                    if(isPathing){//if pathing, override
                                        if(pathTimer>0f){
                                            if(!pathChecked){
                                                CheckPath();
                                            }
                                        }else{
                                            isPathing=false;
                                            if(isNavPlan){
                                                if((navPlanActions.Count>0)&&(navPlanActions!=null)){
                                                    Navigate("plan");
                                                }else{
                                                    Navigate();
                                                }
                                            }else{
                                                Navigate();//normal timed navigation
                                            }
                                        }
                                    }else{
                                        if(isNavPlan){
                                            if((navPlanActions.Count>0)&&(navPlanActions!=null)){
                                                Navigate("plan");
                                            }else{
                                                Navigate();
                                            }
                                        }else{
                                            Navigate();//normal timed navigation
                                        }
                                    }
                                }
                                break;
                        }
                        
                    }
                    //Movement Updates
                    if(navTarget.y!=transform.position.y){
                        navTarget.y=transform.position.y;//Remove y positions from navTarget
                    }
                    navDistance=Vector3.Distance(transform.position,navTarget);
                    shootPos=new Vector3(transform.position.x,transform.position.y+shootPosMod,transform.position.z)+(transform.forward*shootPosModForward);
                    obstaclePos=new Vector3(transform.position.x, transform.position.y + 1f+shootPosMod, transform.position.z);
                }
            }
        }
    }

    void FixedUpdate(){
        Profiler.BeginSample("Testing1");
        if(controlScr.isActive){
            if((isAlive)&&(isActive)){
                if(controllerActive){
                    desiredMove=transform.forward;
                    Physics.SphereCast(transform.position,controller.radius,
                                Vector3.down,out hitInfo,controller.height / 2f,
                                Physics.AllLayers,QueryTriggerInteraction.Ignore);
                    desiredMove =Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
                    if(moveHoldTimer>0f){
                        if(moveHoldTimer<=0.5f){
                            moveDir.x = desiredMove.x * moveSpeed*(0.5f-moveHoldTimer);
                            moveDir.z = desiredMove.z * moveSpeed*(0.5f-moveHoldTimer);
                        }else{
                            moveDir.x = 0f;
                            moveDir.z = 0f;
                        }
                        
                    }else{
                        moveDir.x = desiredMove.x * moveSpeed;
                        moveDir.z = desiredMove.z * moveSpeed;
                    }
                    
                    if(isFlying){
                        rayHigh = Physics.Raycast(obstaclePos,transform.forward,out rayHighHit,rayDistForward);
                        rayGround =Physics.Raycast(transform.position,Vector3.down,out rayGroundHit,rayDistDown);
                        rayGroundHigh =Physics.Raycast(transform.position,Vector3.down,out rayGroundHighHit,rayDistDown*2f);
                        if (navDistance > rayDistForward){
                            if(rayHigh){
                                transform.Translate(Vector3.up *moveSpeed*Time.deltaTime,Space.World);
                            }else{
                                transform.Translate(Vector3.forward *moveSpeed*Time.deltaTime,Space.Self);
                                Moving(true);
                                if(rayGround){
                                    //Go up
                                    transform.Translate(Vector3.up *moveSpeed*Time.deltaTime,Space.World);
                                }
                                if(!rayGroundHigh){
                                    //Go down
                                    if(Physics.Raycast(transform.position, Vector3.down, out rayGroundHighHit, rayDistDown * 50f))
                                    {
                                        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
                                    }//Stops from flying into oblivion
                                }
                            }
                        }else{
                            Moving(false);
                        }
                    }else{//!isFlying NOT flying
                        if(!isGrounded){
                            if (inWater)
                            {
                                if (isUnderwater)
                                {
                                    moveDir.y += (UnityEngine.Random.Range(-climbSpeed/2f, climbSpeed / 2f));
                                }
                                else
                                {
                                    moveDir.y += (gravity * Time.deltaTime) / 4f;
                                }
                                
                            }
                            else
                            {
                                moveDir.y += (gravity * Time.deltaTime) * 4f;
                            }
                        }
                        else
                        {
                            if (isUnderwater)
                            {
                                if (inWater)
                                {
                                    moveDir.y += climbSpeed;
                                }
                            }
                        }
                        if(controller.velocity==Vector3.zero){
                            Moving(false);
                        }else{
                            if(navDistance>walkDist){
                                Moving(true);
                                if (footstepTimer > 0f)
                                {
                                    footstepTimer -= Time.deltaTime;
                                }
                            }else{
                                Moving(false);
                            }
                        }
                        if(navDistance>walkDist){
                            rayHigh=Physics.Raycast(obstaclePos,transform.forward,out rayHighHit,rayDistForward);
                            if(rayHigh){
                                obstacleTimer+=2f*Time.deltaTime;
                                if(obstacleTimer>1f){
                                    Navigate("turnaround");
                                }
                                if(!isObstacle){
                                    if(calculatingObstacle){
                                        if(obstacleCalculationComplete){
                                            Navigate("postobstacle");
                                        }else{
                                            moveDir.x=0.5f;
                                            moveDir.z=0.5f;
                                            raySearchLeft=Physics.Raycast(obstaclePos,-(transform.right),out raySearchLeftHit,rayDistForward*raySearchLeftVal);
                                            raySearchRight=Physics.Raycast(obstaclePos,(transform.right),out raySearchRightHit,rayDistForward*raySearchRightVal);
                                            if((raySearchLeft)&&(raySearchRight)){
                                                Navigate("turnaround");
                                            }else{
                                                if((!raySearchLeft)&&(!raySearchRight)){
                                                    ranInt=UnityEngine.Random.Range(0,1);
                                                    if(ranInt==0){
                                                        tempNavTarget=transform.position-transform.right*(1f+raySearchLeftVal);
                                                        obstacleCalculationComplete=true;
                                                    }else{
                                                        tempNavTarget=transform.position+transform.right*(1f+raySearchRightVal);
                                                        obstacleCalculationComplete=true;
                                                    }
                                                }else{
                                                    if(!raySearchLeft){
                                                        tempNavTarget=transform.position-transform.right*(1f+raySearchLeftVal);
                                                        obstacleCalculationComplete=true;
                                                    }else{
                                                        raySearchLeftVal/=2f;
                                                    }
                                                    if(!raySearchRight){
                                                        tempNavTarget=transform.position+transform.right*(1f+raySearchRightVal);
                                                        obstacleCalculationComplete=true;
                                                    }else{
                                                        raySearchRightVal/=2f;
                                                    }   
                                                }
                                            }
                                        }
                                    }else{
                                        Navigate("preobstacle");
                                    }
                                }else{
                                    if(!calculatingObstacle){
                                        navTimer-=Time.deltaTime;
                                    }
                                }
                            }else{
                                if(isObstacle){
                                    isObstacle=false;
                                    if(calculatingObstacle){
                                        calculatingObstacle=false;
                                        if(!obstacleCalculationComplete){
                                            obstacleCalculationComplete=true;
                                            Navigate("postobstacle");
                                        }
                                    }
                                }
                                raySearch=Physics.Raycast(obstaclePos,transform.forward,out raySearchHit,rayDistForward/2f);
                                if(raySearch){
                                    Navigate("turnaround");
                                }
                                rayLow=Physics.Raycast(new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z),
                                transform.forward,out rayLowHit,rayDistForward);
                                if(rayLow){
                                    if(isGrounded){
                                        jumpTimer+=1f;
                                        if(jumpTimer>3f){
                                            moveDir.y+=climbSpeed*2f;
                                        }else{
                                            moveDir.y+=climbSpeed;
                                        }
                                    }else{
                                        moveDir.y+=climbSpeed*Time.deltaTime;
                                    }
                                }else{
                                    rayJump=Physics.Raycast(shootPos + transform.forward+Vector3.up,Vector3.down,out rayJumpHit,rayDistDown);
                                    if(!rayJump){
                                        if(isGrounded){
                                            jumpTimer+=1f;
                                            if(jumpTimer>3f){
                                                moveDir.y+=climbSpeed*2f;
                                            }else{
                                                Navigate("turnaround");
                                            }
                                            
                                        }
                                    }
                                }
                                
                            }
                        }else{
                            moveDir.x=0f;
                            moveDir.z=0f;
                            if(navActive){
                                if(isPathing){
                                    if(!pathChecked){
                                        CheckPath();
                                    }
                                }else{
                                    switch(controlNavType){
                                        default:
                                            if(isNavPlan){
                                                if((navPlanActions.Count>0)&&(navPlanActions!=null)){
                                                    Navigate("plan");
                                                }else{
                                                    Navigate();
                                                }
                                            }else{
                                                Navigate();//normal timed navigation
                                            }
                                            break;
                                        case "tagalong":
                                        case "hold":
                                            break;
                                    }
                                }
                            }
                        }
                        if(canMove){
                            colFlags=controller.Move(moveDir*Time.fixedDeltaTime);//Actual movement
                            if(controller.isGrounded){
                                if(!isGrounded){
                                    if(footsteps){
                                        if(isMoving){
                                            if (footstepTimer <= 0f)
                                            {
                                                PlayFootStepAudio();
                                            }
                                        }
                                    }
                                }
                                isGrounded=true;
                            }else{
                                isGrounded=false;
                            }
                        }
                    }
                }else{
                    if(canMove){
                        rayHigh = Physics.Raycast(shootPos,transform.forward,out rayHighHit,rayDistForward);
                        rayGround =Physics.Raycast(transform.position,Vector3.down,out rayGroundHit,rayDistDown);
                        if(rayGround){
                            //Go up
                            transform.Translate(Vector3.up *moveSpeed*Time.deltaTime,Space.World);
                        }else{
                            //Go down
                            transform.Translate(Vector3.down * (moveSpeed/2f)*Time.deltaTime,Space.World);
                        }
                        if (navDistance > rayDistForward){
                            if(rayHigh){
                                transform.Translate(Vector3.up *moveSpeed*Time.deltaTime,Space.World);
                            }else{
                                transform.Translate(Vector3.forward *moveSpeed*Time.deltaTime,Space.Self);
                                Moving(true);
                            }
                        }else{
                            Moving(false);
                        }
                    }
                }
                if(inBattle){
                    if(battleTimer>0){
                        battleTimer-=Time.deltaTime;
                    }else{
                        DrawWeapon(false);
                        inBattle=false;
                        TalkSounds("lostSightSounds");
                        navPlans["patrol"]+=3f;
                        shootDist = 75f;
                        if (objTeam=="animal"){
                            navType="hide";
                        }
                    }
                    if(canAttack){
                        if(attackTimer>0){
                            attackTimer-=Time.deltaTime;
                        }else{
                            Attack();
                        }
                    }
                    if(sightTimer>0){
                        sightTimer-=Time.deltaTime;
                    }
                    if(!canSwap){
                        if(swapTimer>0){
                            swapTimer-=Time.deltaTime;
                        }else{
                            canSwap=true;
                        }
                    }
                    if(canMelee){
                        if(meleeTimer>=0f){
                            meleeTimer-=Time.deltaTime;
                        }else{
                            if(nearestEnemy!=null){
                                if(nearestEnemyDist<=meleeDist){
                                    sightAngle=Vector3.Angle(nearestEnemyPos-shootPos,transform.forward);
                                    if((sightAngle>-170f)&&(sightAngle<170f)){//attacks if in sight
                                        if(!nearestEnemy.gameObject.CompareTag(objTeam)){
                                            Melee();
                                        }
                                        
                                    }
                                }   
                            }
                        }
                    }
                }else{
                    if(attackTimer>0){
                        attackTimer-=Time.deltaTime;
                    }else{
                        Search();
                    }
                }
                if(rotateActive){
                    if(aimTargetOverrideTimer<=0){
                        if(isMoving){
                            if(inBattle){
                                if(fireAimTimer<=0){
                                    aimTarget=navTarget;
                                }
                                else{
                                    aimTarget=lastEnemyPos;
                                }
                            }else{
                                aimTarget=navTarget;
                            }
                        }else{
                            if(inBattle){
                                if(sightTimer>10){
                                    aimTarget=lastEnemyPos;
                                }else{
                                    aimTarget=ranRotateVal;
                                }
                            }else{
                                aimTarget=ranRotateVal;
                            }
                        }
                    }else{
                        aimTarget=aimTargetOverride;
                    }
                    //end if aimtargetoverridetimer<=0, allows for manual setting of aimtarget
                    if ((new Vector3(aimTarget.x, transform.position.y, aimTarget.z) - transform.position) != Vector3.zero){
                        targetRot =Quaternion.LookRotation((new Vector3(aimTarget.x,transform.position.y,aimTarget.z) -transform.position),Vector3.up);
                        if(canRotate){
                            if(Vector3.Distance(transform.position,new Vector3(aimTarget.x, transform.position.y, aimTarget.z))>walkDist){
                                if(isMoving){
                                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, (rotateSpeed/2f) * Time.deltaTime);
                                    Debug.DrawRay(shootPos,transform.forward,Color.green,0.1f);
                                }else{
                                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
                                    Debug.DrawRay(shootPos,transform.forward,Color.yellow,0.1f);
                                }
                            }else{
                                Debug.DrawRay(shootPos,transform.forward,Color.red,0.1f);
                            }
                        }
                    }
                }
                if(shields<maxShields){
                    if(rechargeTimer>0){
                        rechargeTimer-=Time.deltaTime;
                    }else{
                        Recharge();
                    }
                }
            }
        }
        Profiler.EndSample();
    }
    

    private Vector3 ChangeRotateVal(){
        randomAimTimer = UnityEngine.Random.Range(navMin,navMax);
        if((nearPlayer)&&(!inBattle)){
            ranRotateVal = new Vector3(playerObj.transform.position.x,
                transform.position.y,playerObj.transform.position.z);
        }else{
            ranRotateVal = new Vector3(transform.position.x + UnityEngine.Random.Range(-5f, 5f),
                transform.position.y,transform.position.z + UnityEngine.Random.Range(-5f, 5f));
        }
        return ranRotateVal;
    }

    void CheckPath(){
        checkPathTimer=0.1f;
        pathChecked=true;
        if (pathCount > 0){
            if (pathStep < pathCount){
                pathStep += 1;
                pathTimer+=1.5f;
                Navigate("path");
            }else{
                Navigate("finishpath");
                if(isNavPlan){
                    if((navPlanActions.Count>0)&&(navPlanActions!=null)){
                        Navigate("plan");
                    }else{
                        Navigate();
                    }
                }else{
                    Navigate();//normal timed navigation
                }
            }
        }
    }

    void NavMatrix(string nmType, string[] nmLabels, float[] nmWeights){
        //Reset: nmType=set,nav type labels array,weights array
        navMatrixTotal = 0f;
        tempInt=0;
        //Assign nav type labels to actions
        navActions.Clear();
        navDef.Clear();
        foreach (string nmn in nmLabels){
            navActions.Add(nmn);
            tempInt++;
        }
        //Add up the total weight
        foreach (float weight in nmWeights){
            navMatrixTotal += weight;
        }
        for (int nmc = 0; nmc < nmLabels.Length; nmc++){
            if (nmWeights[nmc] > 0f){
                navDef[nmLabels[nmc]] = nmWeights[nmc] / navMatrixTotal;
            }else{
                navDef[nmLabels[nmc]] = 0f;
                //navActions.Remove(nmLabels[nmc]);
            }
        }
    }

    string ChoosingNavAction(){
        float ranTotal = 0f;
        float randomNav = UnityEngine.Random.Range(0f, 1f);
        for (int cna = 0; cna < navActions.Count; cna++){
            if ((randomNav > ranTotal) &&(randomNav < (ranTotal + navDef[navActions[cna]]))){
                navAction = navActions[cna];
                return navAction;
            }else{
                ranTotal += navDef[navActions[cna]];
            }
        }
        return navAction;
    }

    void Starving()
    {
        hungerActive = false;
        switch (objName)
        {
            default:
                hp -= 25f;
                hunger += 25f;
                hungerActive = true;
                break;
            case "refugeeCivilian":
                allyTags.Remove("coalition");
                enemyTags.Add("coalition");
                break;

        }
    }

    void ChangeRoutine(string cr="normal")
    {
        routineTimer = 60f;
        switch (cr)
        {
            default:
                curRoutine = cr;
                break;
            case "normal":
                switch (routineName)
                {
                    default:
                        switch (curRoutine)
                        {
                            default:
                                curRoutine = "food";
                                break;
                            case "food":
                                curRoutine = "help";
                                break;
                            case "help":
                                curRoutine = "sleep";
                                break;
                            case "sleep":
                                curRoutine = "food";
                                break;
                        }
                        break;
                    case "civilian":
                        switch (curRoutine)
                        {
                            default:
                                curRoutine = "food";
                                break;
                            case "food":
                                curRoutine = "help";
                                break;
                            case "help":
                                curRoutine = "sleep";
                                break;
                            case "sleep":
                                curRoutine = "food";
                                break;
                        }
                        break;
                    case "refugee":
                        switch (curRoutine)
                        {
                            default:
                                curRoutine = "food";
                                break;
                            case "food":
                                curRoutine = "help";
                                break;
                            case "help":
                                curRoutine = "sleep";
                                break;
                            case "sleep":
                                curRoutine = "food";
                                break;
                        }
                        break;
                }
                break;

        }
        switch(curRoutine){
            default:
                if(myTent!=null){
                    Destroy(myTent.gameObject);
                    myTent=null;
                }
                break;
            case "sleep":
                switch(objTeam){
                    default:
                    
                    break;
                    case "coalition":
                        if(myTent==null){
                            if(controlScr.nfcZeronium>20){
                                if(objName=="refugeeCivilian"){
                                    controlScr.SpawnObj("TentRefugee",transform.position,transform.rotation);
                                }
                                //controlScr.SpawnObj("TentCoalition",transform.position,transform.rotation);
                                controlScr.nfcZeronium-=20;
                            }
                        }
                        break;
                    case "dominion":
                        break;
                    case "zerran":
                        break;
                    case "collective":
                        break;
                    case "refugee":
                        break;
                    case "alharean":
                        break;
                }
                
                break;
        }
        controlNavType = curRoutine;
    }

    void SetNavControl(){
        navGoalMet=false;
        navControl=navControls.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        switch(navControl){
            default:
            case "idle":
                navGoals["routine"]+=1f;
                navGoals["group"]+=1f;
                break;
            case "patrol":
                navGoals["cleararea"]+=1f;
                navGoals["group"]+=1f;
                navGoals["survive"]+=1f;
                break;
            case "patrolleader":
                navGoals["cleararea"]+=1f;
                navGoals["findenemy"]+=1f;
                break;
            case "patrolfollower":
                navGoals["cleararea"]+=1f;
                navGoals["group"]+=1f;
                navGoals["hide"]+=1f;
                break;
            case "routine":
                navGoals["routine"]+=1f;
                break;
            case "combat":
                navGoals["cleararea"]+=1f;
                navGoals["group"]+=1f;
                navGoals["findenemy"]+=1f;
                navGoals["survive"]+=1f;
                break;
            case "combatleader":
                navGoals["cleararea"]+=1f;
                navGoals["hunt"]+=1f;
                break;
            case "combatfollower":
                navGoals["cleararea"]+=1f;
                navGoals["group"]+=2f;
                navGoals["findenemy"]+=1f;
                navGoals["survive"]+=1f;
                break;
        }
        SetNavGoal();
    }

    void CheckGoal(){
        //other goals? alert, ambush,changeweapon,chase,correctposition,melee,cover,dodge,flee,flushout,
        //followwait,lead,investigate,kill,patrol,reactdmg,reactblockedpath,scan,retreatlimited,stunned,suppress,useobj...
        switch(navGoal){
            default:
                navGoalScore+=10f;
                break;
            case "survive":
                if(hp>=maxHp){
                    navGoalScore=navGoalLimit;
                }else if(hp>=(maxHp/2f)){
                    navGoalScore+=20f;
                }
                if(nearAllyCount>nearEnemyCount){
                    navGoalScore+=10f;
                }
                if(nearestEnemyDist>navRange){
                    navGoalScore+=10f;
                }
                break;
            case "hide":
                if(nearAllyCount>nearEnemyCount){
                    navGoalScore+=10f;
                }
                if(nearestEnemyDist>navRange){
                    navGoalScore+=25f;
                }
                if(sightTimer>10f){
                    navGoalScore+=50f;
                }
                break;
            case "hunt":
                if(nearestEnemyDist<navRange){
                    navGoalScore+=10f;
                }
                if(sightTimer<=5f){
                    navGoalScore+=10f;
                }
                if(nearestEnemy==null){
                    navGoalScore+=50f;
                }
                break;
            case "findenemy":
                if(sightTimer<10f){
                    navGoalScore+=25f;
                }
                if(nearestEnemyDist<navRange){
                    navGoalScore+=25f;
                }
                if(nearEnemyCount>0){
                    navGoalScore+=10f;
                    if(nearEnemyCount>2){
                        navGoalScore+=25f;
                    }
                }
                break;
            case "cleararea":
                if(nearEnemyCount==0){
                    navGoalScore=navGoalLimit;
                }else if (nearEnemyCount<3){
                    navGoalScore+=25f;
                }else{
                    CheckRelativePosition();
                }
                break;
            case "group":
                if(isLeader){
                    if(hasSquad){
                        navGoalScore+=50f;
                    }
                }else if (isFollower){
                    if(hasLeader){
                        navGoalScore+=25f;
                    }
                }
                if(nearAllyCount>nearEnemyCount){
                    if(nearAllyCount>5){
                        navGoalScore+=50f;
                    }else if(nearAllyCount>2){
                        navGoalScore+=25f;
                    }else{
                        navGoalScore+=10f;
                    }
                }else{
                    if(nearAllyCount>5){
                        navGoalScore+=100f;
                    }else if(nearAllyCount>2){
                        navGoalScore+=50f;
                    }else{
                        navGoalScore+=25f;
                    }
                }
                break;
            case "routine":
                if(hasRoutine){
                    switch(curRoutine){
                        default:
                            navGoalScore+=10f;
                            break;
                    }
                }else{
                    navGoalScore=navGoalLimit;
                }
                break;
            case "findweaponranged":
                switch(curWep){
                    default:
                        navGoalScore+=10f;
                        break;
                    case 0:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        navGoalScore=navGoalLimit;
                        break;
                }
                break;
            case "findweaponclose":
                switch(curWep){
                    default:
                        navGoalScore+=10f;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 8:
                        navGoalScore=navGoalLimit;
                        break;
                }
                break;
        }
        if(navGoalScore>=navGoalLimit){
            navGoalMet=true;
        }
    }

    void SetNavGoal(){
        navGoal=navGoals.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        navGoalScore=0f;
        foreach(string nps in navPlansList){
            navPlans[nps]=0;
        }
        switch(navGoal){
            default:
                
                break;
            case "survive":
                navPlans["evade"]+=1f;
                navPlans["groupup"]+=1f;
                break;
            case "cleararea":
                CheckRelativePosition();
                navPlans["pursue"]+=1f;
                navPlans["search"]+=1f;
                break;
            case "group":
                navPlans["groupup"]+=1f;
                navPlans["search"]+=1f;
                break;
            case "routine":
                navPlans["routine"]+=1f;
                break;
            case "hide":
                navPlans["evade"]+=1f;
                break;
            case "hunt":
                navPlans["pursue"]+=2f;
                break;
            case "findweaponranged":
                navPlans["findweaponranged"]+=1f;
                navPlans["search"]+=1f;
                break;
            case "findweaponclose":
                navPlans["findweaponclose"]+=1f;
                navPlans["search"]+=1f;
                break;
            case "findenemy":
                navPlans["search"]+=2f;
                navPlans["pursue"]+=2f;
                break;
        }
        if(!isNavPlan){
            StartCoroutine(SetNavPlan());
        }
    }

    void CheckRelativePosition(){
        relativePos=transform.InverseTransformPoint(lastEnemyPos);
        if(relativePos.x>0){
            navPlans["flankright"]+=1f;
        }else{
            navPlans["flankleft"]+=1f;
        }
    }

    void DodgeRelativePosition(Vector3 dodgePos){
        navOverrideTimer=1f;
        relativePos=transform.InverseTransformPoint(dodgePos);
        if(relativePos.x>0){
            Navigate("dodgeright");
        }else{
            Navigate("dodgeleft");
        }
    }

    void CheckRelativePlayer(){
        if(playerObj!=null){
            relativePosPlayer=playerObj.transform.InverseTransformPoint(transform.position);
        }
        if(relativePosPlayer.x>0){
            navPlans["flankleft"]+=1f;
        }else{
            navPlans["flankright"]+=1f;
        }
    }

    IEnumerator SetNavPlan(float pauseDelay=0f){
        navTimer+=pauseDelay;
        yield return new WaitForSeconds(pauseDelay);
        isNavPlan=false;
        navPlanStep=0;
        navPlanActions.Clear();
        CheckGoal();
        if(navGoalMet){
            navControls[navControl]/=2f;
            navGoals[navGoal]/=2f;
            SetNavControl();
            yield break;
        }
        string npitxt=objName+" ";
        /*
        for(int npi=0;npi<navPlansList.Count;npi++){
            npitxt+=navPlansList[npi]+" "+navPlans[navPlansList[npi]];
        }*/
        navPlan=navPlans.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        npitxt+=" PLAN "+navPlan;
        //Debug.Log(npitxt);
        switch(navPlan){
            default:
                TalkSounds("random");
                navPlanActions.Add("idle");
                navPlanActions.Add("random");
                break;
            case "routine":
                navPlanActions.Add("origin");
                navPlanActions.Add("idle");
                navPlanActions.Add("random");
                navPlanActions.Add("idle");
                TalkSounds("random");
                break;
            case "flankleft":
                TalkSounds("flankleft");
                navPlanActions.Add("flankleft");
                navPlanActions.Add("follow");
                navPlanActions.Add("flankleft");
                navPlanActions.Add("follow");
                break;
            case "flankright":
                TalkSounds("flankright");
                navPlanActions.Add("flankright");
                navPlanActions.Add("follow");
                navPlanActions.Add("flankright");
                navPlanActions.Add("follow");
                break;
            case "groupup":
                TalkSounds("groupupSounds");
                navPlanActions.Add("help");
                navPlanActions.Add("random");
                navPlanActions.Add("help");
                break;
            case "evade":
                navPlanActions.Add("cover");
                navPlanActions.Add("random");
                navPlanActions.Add("cover");
                break;
            case "pursue":
                TalkSounds("tauntSounds");
                navPlanActions.Add("follow");
                navPlanActions.Add("random");
                navPlanActions.Add("follow");
                break;
            case "search":
                TalkSounds("tauntSounds");
                navPlanActions.Add("follow");
                navPlanActions.Add("random");
                break;
            case "flushout":
                TalkSounds("flushoutSounds");
                if((greCount>0)&&(attackChanceGrenade>1)){
                    attackChanceGrenade-=1;
                }else{
                    navPlans["flushout"]-=1f;
                    CheckRelativePosition();
                }
                break;
            case "findweaponranged":
                navPlanActions.Add("weapon");
                navPlanActions.Add("follow");
                navPlanActions.Add("random");
                break;
            case "findweaponclose":
                navPlanActions.Add("weapon");
                navPlanActions.Add("follow");
                navPlanActions.Add("random");
                break;
            case "followpath":
                navPlanActions.Add("path");
                break;
            case "leadgroup":
                navPlanActions.Add("follow");
                navPlanActions.Add("follow");
                navPlanActions.Add("random");
                break;
            case "followgroup":
                navPlanActions.Add("help");
                navPlanActions.Add("help");
                navPlanActions.Add("help");
                break;
            case "patrol":
                navPlanActions.Add("random");
                navPlanActions.Add("lookaround");
                navPlanActions.Add("random");
                navPlanActions.Add("lookaround");
                break;
            case "reactdamage":
                navPlanActions.Add("cover");
                navPlanActions.Add("help");
                navPlanActions.Add("cover");
                break;
            case "blocked":
                navPlanActions.Add("random");
                navPlanActions.Add("random");
                navPlanActions.Add("random");
                break;
            case "door":
                break;
            case "reposition":
                break;
            case "findpath":
                navPlanActions.Add("findpath");
                break;
            
        }
        navPlanStepCount=navPlanActions.Count;
        isNavPlan=true;
    }

     void Navigate(string nt="normal"){//Navigation
        if(navPlans["blocked"]>1f){
            navPlans["blocked"]-=0.1f;
        }
        if(navPlans["findpath"]>0f){
            navPlans["findpath"]-=0.1f;
        }
        if (inWater)
        {
            if(isUnderwater){
                moveDir.y = climbSpeed;
            }else{
                moveDir.y = climbSpeed/2f;
            }
            
        }
        if (inBattle)
        {
            navTimer= UnityEngine.Random.Range(navMin, navMax);
        }
        else
        {
            navTimer= UnityEngine.Random.Range(navMin, navMax*2f);
        }
        switch(nt){//Determines likelihood of each action. Anything other than normal for override.
            default:
                navAction=nt;
                break;
            case "normal":
                navAction=ChoosingNavAction();
                break;
            case "reset":
                isObstacle=false;
                navTarget=finalNavTarget;
                navTimer=finalNavTimer;
                navOverrideTimer=0f;
                impulseTimer=0f;
                break;
            case "tagalong":
                navAction=ChoosingNavAction();
                break;
            case "plan":
                if(navPlanStepCount>navPlanActions.Count){
                    navPlanStepCount=0;
                    Navigate();
                }else{
                    if(navPlanStep<navPlanStepCount){
                        navAction=navPlanActions[navPlanStep];
                        navPlanStep+=1;
                    }else{
                        StartCoroutine(SetNavPlan(UnityEngine.Random.Range(navMin,navMax)));
                    }
                }
                break;
        }
        if(!string.IsNullOrEmpty(controlNavType)){
            switch(controlNavType){//If anything other than "none" it overrides all prior behavior to go to that point
                default:
                    if(!isPathing){
                        if(GameObject.Find(controlNavType)!=null){
                            navTargetObj=GameObject.Find(controlNavType);
                            navTargetObjDist=Vector3.Distance(transform.position,navTargetObj.transform.position);
                            if(navTargetObjDist>rayDistForward){
                                navAction="target";
                            }
                        }else{
                            ChangeNavSettings("none","none");
                        }
                    }
                    break;
                case "ally":
                    if(controlScr.gameMode=="campaign"){
                        if(!inBattle){
                            ranInt=UnityEngine.Random.Range(0,3);
                            if(ranInt==0){
                                navType="origin";
                            }
                        }//confine these types of chars to their spawn areas
                    }
                    
                    break;
                case "enemy":
                    if(controlScr.gameMode=="campaign"){
                        if(!inBattle){
                            ranInt=UnityEngine.Random.Range(0,3);
                            if(ranInt==0){
                                navType="origin";
                            }
                        }//confine these types of chars to their spawn areas
                    }
                    break;
                case "hide":
                    //ignore
                    break;
                case "hold":
                    navAction=controlNavType;
                    navType="none";
                    break;
                case "random":
                    navType="normal";
                    break;
                case "tagalong":
                    if((!navActive)||(navType!="tagalong")){
                        ChangeNavSettings("tagalong","tagalong");
                    }
                    break;
                case "none":
                
                    break;
            }
        }
        //Separate event triggers
        switch(controlNavType){
            default:

                break;
            case "JormadBarracks":
                if(objName=="ZephyrDrakon"){
                    if(controlScr.missionStatus["JormadFollow"]=="following"){
                        playerDistance=Vector3.Distance(transform.position,playerObj.transform.position);
                        if(playerDistance>100f){
                            ChangeRep(0,true);
                            controlScr.UpdateMission("JormadFollow","hostile");
                            controlScr.UpdateMission("ZephyrDrakon","hostile","starthostile");
                        }else if(playerDistance>40f){
                            if(canMove){
                                canMove=false;
                            }
                        }else{
                            if(!canMove){
                                if(!isTalking){
                                    canMove=true;
                                }else{
                                    canMove=false;
                                }
                            }
                            controlScr.missionTargets["JormadFollow"]=transform.position;
                        }
                        if(GameObject.Find("JormadBarracks")!=null){
                            if(navTargetObj!=GameObject.Find("JormadBarracks")){
                                navTargetObj=GameObject.Find("JormadBarracks");
                            }
                        }
                        if(navTargetObj!=null){
                            navTargetObjDist=Vector3.Distance(transform.position,navTargetObj.transform.position);
                            if(navTargetObjDist<10f){
                                if(playerDistance<5f){
                                    if(controlScr.missionStatus["YarahZen"]=="dead"){
                                        controlScr.UpdateMission("ZephyrDrakon","interrogatedead","startdead");
                                        controlScr.UpdateMission("JormadFollow","dead");
                                        myConvoScr.ConvoStart(true);
                                    }else{
                                        if(controlScr.missionStatus["SearchJormad"]=="escape"){
                                            controlScr.UpdateMission("ZephyrDrakon","interrogateescape","startescape");
                                            controlScr.UpdateMission("JormadFollow","escape");
                                            myConvoScr.ConvoStart(true);
                                        }else{
                                            controlScr.UpdateMission("ZephyrDrakon","interrogate","interrogate");
                                            controlScr.UpdateMission("JormadFollow","interrogate");
                                            myConvoScr.ConvoStart(true);
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    if(controlScr.missionStatus["JormadFollow"]=="stumbleupon"){
                        if(GameObject.Find("JormadBarracks")!=null){
                            if(navTargetObj!=GameObject.Find("JormadBarracks")){
                                navTargetObj=GameObject.Find("JormadBarracks");
                            }
                        }
                        if(navTargetObj!=null){
                            playerDistance=Vector3.Distance(transform.position,playerObj.transform.position);
                            navTargetObjDist=Vector3.Distance(transform.position,navTargetObj.transform.position);
                            if(navTargetObjDist<10f){
                                if(canMove){
                                    canMove=false;
                                }
                                if(playerDistance<5f){
                                    if(controlScr.missionStatus["YarahZen"]=="dead"){
                                        controlScr.UpdateMission("ZephyrDrakon","interrogatedead","startdeadstumble");
                                        controlScr.UpdateMission("JormadFollow","dead");
                                        myConvoScr.ConvoStart(true);
                                    }else{
                                        if(controlScr.missionStatus["SearchJormad"]=="escape"){
                                            controlScr.UpdateMission("ZephyrDrakon","interrogateescape","startescapestumble");
                                            controlScr.UpdateMission("JormadFollow","escape");
                                            myConvoScr.ConvoStart(true);
                                        }else{
                                            controlScr.UpdateMission("ZephyrDrakon","interrogate","interrogatestumble");
                                            controlScr.UpdateMission("JormadFollow","interrogate");
                                            myConvoScr.ConvoStart(true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
        }
        //Specific overrides
        if(holdingFlag){
            ranInt=UnityEngine.Random.Range(0,5);
            if(ranInt>0){
                navAction="flag";
            }
        }
        switch(navAction){//Actual navigation actions.
            default:
            case "lookaround":
                ChangeRotateVal();
                break;
            case "idle":

                break;
            case "lookleft":
                aimTargetOverride=transform.forward-transform.right;
                aimTargetOverrideTimer+=1f;
                break;
            case "lookright":
                aimTargetOverride=transform.forward+transform.right;
                aimTargetOverrideTimer+=1f;
                break;
            case "turnaround":
                navTarget=transform.position-transform.forward+new Vector3(UnityEngine.Random.Range(-navRange, navRange),
                        0f,UnityEngine.Random.Range(-navRange, navRange));
                isObstacle=false;
                calculatingObstacle=false;
                obstacleCalculationComplete=true;
                break;
            case "lookplayer":
                if(playerObj!=null){
                    aimTargetOverride=playerObj.transform.position;
                    aimTargetOverrideTimer+=1f;
                }
                break;
            case "flankleft":
                if(inBattle){
                    if(nearestEnemy!=null){
                        transform.LookAt(lastEnemyPos);
                    }
                }
                navTarget=transform.position-(transform.right*5f)+transform.forward;
                break;
            case "flankright":
                if(inBattle){
                    if(nearestEnemy!=null){
                        transform.LookAt(lastEnemyPos);
                    }
                }
                navTarget=transform.position+(transform.right*5f)+transform.forward;
                break;
            case "left":
                navTarget=transform.position-(transform.right*3f);
                break;
            case "right":
                navTarget=transform.position+(transform.right*3f);
                break;
            case "dodgeleft":
                finalNavTarget=navTarget;
                finalNavTimer=navTimer;
                navTarget=transform.position-(transform.right*1f);
                aimTargetOverrideTimer+=1f;
                aimTargetOverride=lastEnemyPos;
                navTimer=0.5f;
                break;
            case "dodgeright":
                finalNavTarget=navTarget;
                finalNavTimer=navTimer;
                navTarget=transform.position-(transform.right*1f);
                aimTargetOverrideTimer+=1f;
                aimTargetOverride=lastEnemyPos;
                navTimer=0.5f;
                break;
            case "random":
                navTarget=new Vector3(transform.position.x +UnityEngine.Random.Range(-navRange, navRange),
                        transform.position.y,transform.position.z +UnityEngine.Random.Range(-navRange, navRange));
                break;
            case "obstaclejump":

                break;
            case "preobstacle":
                navPlans["blocked"]+=0.1f;
                if(!isFlying){
                    navPlans["findpath"]+=0.2f;
                }
                finalNavTimer=navTimer;
                finalNavTarget=navTarget;
                obstacleCalculationComplete=false;
                calculatingObstacle=true;
                raySearchLeftVal=rayDistForward*2f;
                raySearchRightVal=rayDistForward*2f;
                break;
            case "postobstacle":
                calculatingObstacle=false;
                navTarget=tempNavTarget;
                navTimer=1f;
                isObstacle=true;
                break;
            case "path":
                if(pathList!=null){
                    navTarget=pathList[pathStep];
                }
                break;
            case "findpath":
                if((!isPathing)&&(navTarget!=null)&&(!isFlying))
                {
                    switch(controlNavType){
                        default:
                            SetPathList(navTarget);
                            break;
                        case "none":
                        case "tagalong":
                        case "hold":    
                            navPlans["findpath"]=0;
                            break;
                    }
                    
                }
                break;
            case "target":
                if(navTargetObj!=null){
                    if(!isPathing){
                        navTarget=navTargetObj.transform.position;
                    }
                }
                break;
            case "flag":
                if(navTargetObj!=null){
                    if(holdingFlag){
                        switch(objTeam){
                            default:
                                navTarget=originPos;
                                break;
                            case "coalition":
                                navTarget=controlScr.coalitionSpawnPos;
                                break;
                            case "dominion":
                                navTarget=controlScr.dominionSpawnPos;
                                break;
                        }
                    }else{
                        navTarget=navTargetObj.transform.position;
                    }
                }else{
                    navTarget=originPos;
                }
                break;
            case "finishpath":
                navTarget=pathNavTarget;
                //DEBUG?
                isPathing=false;
                break;
            case "special"://allows for routines
                switch(objName){
                    default:
                        navTarget=FindNearest("interactive").transform.position;
                        break;
                }
                break;
            case "weapon":
                if(GameObject.FindGameObjectsWithTag("weapon")!=null){
                    navTarget=FindNearest("weapon").transform.position;
                }else{
                    navTimer=0f;
                }
                break;
            case "cover":
                navTarget=FindNearest("cover").transform.position;
                if(inBattle){
                    if(nearestEnemy!=null){
                        if(canSeeEnemy){
                            if(lastEnemyPos.x<transform.position.x){
                                navTarget.x += 2f;
                            }else{
                                navTarget.x -= 2f;
                            }
                            if(lastEnemyPos.z<transform.position.z){
                                navTarget.z += 2f;
                            }else{
                                navTarget.z -= 2f;
                            }
                        }else{
                            if(sightTimer<5f){
                                if(nearAllyCount>=nearEnemyCount){
                                    navGoals["hunt"]+=1f;
                                }else{
                                    navPlans["patrol"]+=1f;
                                }
                            }
                        }
                    }
                }
                break;
            case "retreat":
            case "origin":
            case "hold":
                navTarget=originPos;
                break;
            case "tagalong":
                playerDistance=Vector3.Distance(transform.position,playerObj.transform.position);
                if (playerDistance >10f){
                    navTarget=new Vector3(playerObj.transform.position.x+UnityEngine.Random.Range(-5f,5f),playerObj.transform.position.y,
                    playerObj.transform.position.z+UnityEngine.Random.Range(-5f,5f));
                }else{
                    if(inBattle){
                        navTarget=new Vector3(transform.position.x +UnityEngine.Random.Range(-navRange, navRange),
                        transform.position.y,transform.position.z +UnityEngine.Random.Range(-navRange, navRange));
                    }else{
                        navTarget=transform.position;
                    }
                }
                break;
            case "follow":
                if(nearestEnemy==null){
                    nearestEnemy=FindNearest("enemy");
                }
                if (nearestEnemy != null){
                    if(nearEnemyCount<nearAllyCount){
                        TalkSounds("followSounds");
                    }
                    navTarget=nearestEnemy.transform.position;
                }else{
                    navTarget=
                        new Vector3(transform.position.x +
                            UnityEngine.Random.Range(-navRange, navRange),
                            transform.position.y,
                            transform.position.z +
                            UnityEngine.Random.Range(-navRange, navRange));
                }
                break;
            case "help":
                if (nearestAlly == null){
                    nearestAlly = FindNearest("ally");
                }
                if (nearestAlly != null){
                    if((isFollower)&&(hasLeader)){
                        if(leaderObj!=null){
                            navTarget=(leaderObj.transform.position + transform.position) / 2f;
                        }else{
                            hasLeader=false;
                        }
                    }else{
                        navTarget=(nearestAlly.transform.position + transform.position) / 2f;
                    }
                    if(inBattle){
                        if(sightTimer>0){
                            TalkSounds("helpSounds");
                        }
                    }
                }else{
                    navTarget=
                        new Vector3(transform.position.x +
                            UnityEngine.Random.Range(-navRange, navRange),
                            transform.position.y,
                            transform.position.z +
                            UnityEngine.Random.Range(-navRange, navRange));
                }
                break;
            case "none":
                navTarget=transform.position;
                break;
        }
        navDistance=Vector3.Distance(transform.position,navTarget);
        if(!isObstacle){
            navTimer += 0.1f+(navDistance/moveSpeed);
            if(navTimer>navMax){
                navTimer=navMax;
            }
            if(navDistance>navRange){
                if(!isPathing){
                    if(!isFlying){
                        if(Physics.Linecast(shootPos,new Vector3(navTarget.x,navTarget.y+shootPosMod,navTarget.z))){
                            switch(controlNavType){
                                default:
                                    SetPathList(navTarget);
                                    break;
                                case "none":
                                case "hide":
                                case "tagalong":
                                    break;
                            }
                        }
                    }
                }
            }
        }
     }

     public void SetPathList(Vector3 spTarget){
        //navScaleCount changes size of grid and number of node points.
        Profiler.BeginSample("setpathlist");
        //Determine if indoors
        rayIndoors=Physics.Raycast(shootPos+(Vector3.up*shootPosMod),Vector3.up,out rayIndoorsHit,10f);
        if(rayIndoors){
            //determine if indoors by looking for a ceiling
            isIndoors=true;
        }else{
            isIndoors=false;
        }
        navPathRange=Vector3.Distance(transform.position,spTarget);
        navScaleCount=Mathf.RoundToInt(5+(navPathRange/navRange));
        if(navScaleCount>10){
            navScaleCount=10;
        }
        totalPathDist=0f;
        navScale=navPathRange/navScaleCount;
        tempNavPosition=transform.position;
        navPositions.Clear();
        pathList.Clear();
        bool complete = false;
        int failSafe=50;
        float posDistOrig=1000f;
        float posDistFinal=1000f;
        float posDistTotal=1000f;
        float origDist=1000f;
        float minDistFinal=1000f;
        float minDistOrig=1000f;
        float minDistTotal=1000f;
        float baseHeight=transform.position.y;
        float heightLimit=baseHeight+climbSpeed;
        Vector3 curNavPosition=transform.position;
        Vector3 origPos=transform.position;
        Vector3 finalPos=transform.position;
        //Generate grid. If indoors use prebuilt paths, otherwise generate one.
        if((GameObject.FindGameObjectsWithTag("navnode")!=null)&&(isIndoors||manualPathOverride)){
            foreach(GameObject gonn in GameObject.FindGameObjectsWithTag("navnode")){
                tempNavX=gonn.transform.position.x;
                tempNavZ=gonn.transform.position.z;
                tempNavY=gonn.transform.position.y+shootPosMod;
                tempNavPosition=new Vector3(tempNavX,tempNavY,tempNavZ);
                if(Vector3.Distance(transform.position,tempNavPosition)<navPathRange){
                    navPositions.Add(tempNavPosition);
                }
            }
        }else{
            for(int pl=0;pl<navScaleCount*2;pl++){
                for(int plr=0;plr<navScaleCount*2;plr++){
                    //Start at radius of half navdistance to left, work right and down x,z coordinates
                    tempNavX=transform.position.x-navPathRange+(pl*navScale);
                    tempNavZ=transform.position.z-navPathRange+(plr*navScale);
                    tempNavPosition.x=tempNavX;
                    tempNavPosition.z=tempNavZ;
                    tempNavY=transform.position.y;
                    if(Terrain.activeTerrain!=null){
                        tempNavY=Terrain.activeTerrain.SampleHeight(tempNavPosition)+shootPosMod;
                    }
                    tempNavPosition.y=tempNavY;
                    if(tempNavPosition.y<=heightLimit){
                        navPositions.Add(tempNavPosition);
                    }
                }
            }
        }
        foreach(Vector3 tnp in navPositions){
            posDistOrig=Vector3.Distance(transform.position,tnp);
            posDistFinal=Vector3.Distance(spTarget,tnp);
            //Origin point
            if(posDistOrig<=minDistOrig){
                origPos=tnp;
                minDistOrig=posDistOrig;
            }
            //Final point closest
            if(posDistFinal<=minDistFinal){
                finalPos=tnp;
                minDistFinal=posDistFinal;
            }
        }
        pathList.Add(origPos);
        totalPathDist+=minDistFinal;
        curNavPosition=origPos;
        while (!complete){
            failSafe -= 1;
            if (failSafe <= 0){
                complete = true;
                navAction = "random";
            }else{
                foreach(Vector3 tnp in navPositions){
                    if(Vector3.Distance(tnp,curNavPosition)>1f){
                        posDistOrig=Vector3.Distance(curNavPosition,tnp);
                        posDistFinal=Vector3.Distance(spTarget,tnp);
                        posDistTotal=posDistOrig+posDistFinal;
                        if(posDistFinal<=origDist){
                            posDistTotal*=0.8f;
                        }
                        if(Physics.Linecast(curNavPosition,tnp)){
                            posDistTotal*=3f;
                        }
                        if (posDistTotal < minDistTotal){
                            if(Vector3.Distance(tnp,curNavPosition)>1f){
                                minDistTotal = posDistTotal;
                                tempNavPosition = tnp;
                            }
                        }
                    }
                }
                if(objName=="ZephyrDrakon"){
                    Debug.DrawLine(curNavPosition,tempNavPosition,Color.magenta,30f);
                }
                if(Vector3.Distance(tempNavPosition,curNavPosition)>1f){
                    curNavPosition=tempNavPosition;
                    pathList.Add(curNavPosition);
                    totalPathDist+=minDistTotal;
                    minDistFinal=navPathRange;
                    minDistTotal=navPathRange;
                }
                if(Vector3.Distance(curNavPosition,finalPos)<navScale){
                    complete = true;
                }else{

                }
                navPositions.Remove(curNavPosition);
            }
        }
        if (pathList.Count > 1){
            pathCount = pathList.Count - 1;
            pathStep = 0;
            nextPathStep = 0;
            isPathing=true;
            pathTimer=2f+(totalPathDist/100f);
            pathNavTarget=spTarget;
        }else{
            navTarget=pathNavTarget;
            isPathing=false;
            nextPathStep = 0;
            pathStep = 0;
            pathCount = 0;
        }
        Profiler.EndSample();
     }

     void Alert(){
        if(!inBattle){
            inBattle=true;
            if(objName=="alhareanCruiser"){
                 controlScr.SpawnObj("alhareanSoldier",new Vector3(transform.position.x,transform.position.y-shootPosMod,transform.position.z),transform.rotation,false);
            }
            if(shootDist<150f){
                shootDist = 150f;
            }else{
                if(shootDist<250f){
                    shootDist += 50f;
                }
            }
            switch(controlScr.gameMode){
                default:
                case "campaign":
                    battleTimer=60f;
                    break;
                case "multiplayer":
                case "Deathmatch":
                case "Capture the flag":
                    battleTimer=300f;
                    break;
            }
            if(isLeader){
                navControls["combatleader"]+=1f;
            }else if(isFollower){
                navControls["combatfollower"]+=1f;
            }else{
                navControls["combat"]+=1f;
            }
        }
        DrawWeapon(true);
        sightTimer=15f;
        for (int i = 0; i < allyTags.Count; i++){
            foreach (GameObject ally in GameObject.FindGameObjectsWithTag(objTeam)){
                if (ally.GetComponent<NewNpcScr>() != null){
                    if (!ally.GetComponent<NewNpcScr>().inBattle){
                        if (Vector3.Distance(transform.position, ally.transform.position) < shootDist){
                            ally.GetComponent<NewNpcScr>().battleTimer=60f;
                            ally.GetComponent<NewNpcScr>().inBattle=true;
                        }
                    }
                }
            }
        }
     }
     
     void Search(){
        attackTimer=UnityEngine.Random.Range(attackMin,attackMax);
        if(nearestEnemy==null){
            navPlans["patrol"]+=1f;
            nearestEnemy=FindNearest("enemy");
        }else{
            if(enemySearchTimer<=0){
                navPlans["search"]+=1f;
                nearestEnemy=FindNearest("enemy");
                enemySearchTimer=5f;
            }
        }
        if(nearestEnemy!=null){
            lastEnemyPos=nearestEnemy.transform.position;
            ranInt=UnityEngine.Random.Range(0,searchChanceMax);
            if(ranInt>0){
                if(nearestEnemyDist<shootDist){
                    nearestEnemyPos=new Vector3(nearestEnemy.transform.position.x,
                        nearestEnemy.transform.position.y+UnityEngine.Random.Range(0,3f),
                        nearestEnemy.transform.position.z);
                    rayAim=Physics.Raycast(shootPos,(nearestEnemyPos-shootPos),out rayAimHit,shootDist);
                    if((rayAim)&&(rayAimHit.transform!=null)){
                        if(rayAimHit.transform.gameObject==nearestEnemy){
                            sightAngle=Vector3.Angle(nearestEnemyPos-shootPos,transform.forward);
                            if((sightAngle>-170f)&&(sightAngle<170f)){//search
                                lastEnemyPos=nearestEnemyPos;
                                if(!inBattle){
                                    TalkSounds("sightSounds");
                                }
                                Alert();
                            }
                        }
                    }
                }
            }
        }
     }
     void Attack(){
        if((objName=="coalitionTank")||(objName=="dominionTank")){
            if(curWep!=101){
                curWep=101;
            }
        }
        attackTimer=UnityEngine.Random.Range(attackMin,attackMax);
        if(nearestEnemy==null){
            canSeeEnemy=false;
            nearestEnemy=FindNearest("enemy");
        }else{
            if(enemySearchTimer<=0){
                nearestEnemy=FindNearest("enemy");
                enemySearchTimer=3f;
            }
        }
        if((nearestEnemy!=null)&&(nearestEnemy!=this.gameObject)){//Call again after setting nearestEnemy
            lastEnemyPos=nearestEnemy.transform.position;
            if(isMoving){
                ranInt=UnityEngine.Random.Range(0,2);
            }else{
                ranInt=UnityEngine.Random.Range(0,attackChanceMax);
            }
            if(hitScore>0){
                navPlans["pursue"]+=(hitScore/100f);
            }
            if(ranInt>0){
                nearestEnemyPos =
                        new Vector3(nearestEnemy.transform.position.x,
                            nearestEnemy.transform.position.y +UnityEngine.Random.Range(0,2f),
                            nearestEnemy.transform.position.z);
                nearestEnemyDist =Vector3.Distance(shootPos, nearestEnemyPos);
                if(nearestEnemyDist<shootDist){
                    rayAim=Physics.Raycast(shootPos,(nearestEnemyPos-shootPos),out rayAimHit,shootDist);
                    if((rayAim)&&(rayAimHit.transform!=null)){
                        if(rayAimHit.transform.gameObject==nearestEnemy){
                            sightAngle=Vector3.Angle(nearestEnemyPos-shootPos,transform.forward);
                            if((sightAngle>-170f)&&(sightAngle<170f)){//attacks if in sight
                                canSeeEnemy=true;
                                lastEnemyPos=nearestEnemyPos;
                                Alert();
                                if(isMoving){
                                    fireAimTimer=0.25f;
                                    attackTimer+=0.1f;
                                }else{
                                    fireAimTimer=1f;
                                }
                                enemySearchTimer+=0.5f;
                                if (nearestEnemyDist>meleeDist){
                                    if((greCount>0)&&(nearEnemyCount>1)){
                                        ranInt=UnityEngine.Random.Range(0,attackChanceGrenade);
                                        if(ranInt==0){
                                            Grenade();
                                        }
                                    }
                                    if(canShoot){
                                        if(ammo>0){
                                            Shoot();
                                        }else{
                                            StartCoroutine(Reload());
                                        }
                                    }
                                }
                            }
                            UpdateRecentEnemies(nearestEnemy,0.5f);
                        }else{
                            canSeeEnemy=false;
                            if(sightTimer>10f){//shoots blindly for a few seconds after sighting
                                ranInt=UnityEngine.Random.Range(0,attackChanceMax);
                                if(ranInt==0){
                                    Shoot();
                                }
                                navPlans["flushout"]+=2f;
                                CheckRelativePosition();
                            }else if(sightTimer>5f){
                                navPlans["search"]+=1f;
                                if(isLeader&&hasSquad){
                                    navPlans["flushout"]+=1f;
                                    navGoals["hunt"]+=1f;
                                    foreach(GameObject sqo in squadObjs){
                                        sqo.GetComponent<NewNpcScr>().CheckRelativePosition();
                                        navPlans["pursue"]+=1f;
                                        navGoals["findenemy"]+=1f;
                                    }
                                }
                            }else{
                                navPlans["flushout"]+=1f;
                                CheckRelativePosition();
                                if(isLeader){
                                    navPlans["pursue"]+=1f;
                                }
                            }
                        }
                    }
                   
                }
            }
        }
     }

     public void UpdateRecentEnemies(GameObject recEn,float recAmt){
        if(recAmt>0){
            if((recentEnemies!=null)&&(recentEnemies.Count>0)){
                if(recentEnemies.Contains(recEn)){
                    recentEnemyScores[recEn]+=recAmt;
                }else{
                    recentEnemies.Add(recEn);
                    recentEnemyScores.Add(recEn,recAmt);
                }
            }else{
                recentEnemies.Add(recEn);
                recentEnemyScores.Add(recEn,recAmt);
            }
        }else{
            if(recentEnemies.Contains(recEn)){
                recentEnemies.Remove(recEn);
                recentEnemyScores.Remove(recEn);
            }
        }
        if((recentEnemies!=null)&&(recentEnemies.Count>0)){
            nearestEnemy=recentEnemyScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            curThreatScore=recentEnemyScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Value;
            if(curThreatScore>threatScore){
                if((greCount>0)&&(attackChanceGrenade>1)){
                    attackChanceGrenade-=1;
                }
            }
            if(nearestEnemy!=null){
                nearestEnemyPos=new Vector3(nearestEnemy.transform.position.x,
                        nearestEnemy.transform.position.y+UnityEngine.Random.Range(0,2f),
                        nearestEnemy.transform.position.z);
                nearestEnemyDist =Vector3.Distance(shootPos, nearestEnemyPos);
            }
            if((isLeader)&&(hasSquad)){
                if((squadObjs.Count>0)&&(squadObjs!=null)){
                    foreach(GameObject sqo in squadObjs){
                        if(sqo.GetComponent<NewNpcScr>()!=null){
                            sqo.GetComponent<NewNpcScr>().nearestEnemy=nearestEnemy;
                            TalkSounds("focusfireSounds");
                            //Tell squad to concentrate fire
                        }
                    }
                }
            }
        }
     }

     public void UpdateRecentAllies(GameObject recAl,float recAmt){
        if(recAmt>0){
            if((recentAllies!=null)&&(recentAllies.Count>0)){
                if(recentAllies.Contains(recAl)){
                    recentAllyScores[recAl]+=recAmt;
                }else{
                    recentAllies.Add(recAl);
                    recentAllyScores.Add(recAl,recAmt);
                }
            }else{
                recentAllies.Add(recAl);
                recentAllyScores.Add(recAl,recAmt);
            }
        }else{
            if(recentAllies.Contains(recAl)){
                recentAllies.Remove(recAl);
                recentAllyScores.Remove(recAl);
            }
        }
        if((recentAllies!=null)&&(recentAllies.Count>0)){
            nearestAlly=recentAllyScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        }
        if(isFollower){
            if(hasLeader){
                if(leaderObj==null){
                    hasLeader=false;
                }
            }else{
                if(nearestAlly.GetComponent<NewNpcScr>()!=null){
                    if(nearestAlly.GetComponent<NewNpcScr>().isLeader){
                        hasLeader=true;
                        leaderObj=nearestAlly;
                        if(!nearestAlly.GetComponent<NewNpcScr>().hasSquad){
                            nearestAlly.GetComponent<NewNpcScr>().UpdateSquad(this.gameObject,true);
                        }
                    }
                }
            }
        }
     }

     public void UpdateSquad(GameObject sqObj,bool addSq){
        if(addSq){
            if(!hasSquad){
                hasSquad=true;
            }
            if(!squadObjs.Contains(sqObj)){
                squadObjs.Add(sqObj);
            }
        }else{
            if((squadObjs.Count>0)&&(squadObjs!=null)){
                if(squadObjs.Contains(sqObj)){
                    squadObjs.Remove(sqObj);
                    if((squadObjs.Count==0)||(squadObjs==null)){
                        hasSquad=false;
                    }
                }
            }
        }
     }

     GameObject FindNearest(string resourceType){
        resourceDist=Mathf.Infinity;
        nearestResourceDist=resourceDist;
        nearestResource=null;
        //hitColliders = Physics.OverlapSphere(transform.position, shootDist);
        //nearObjs.Clear();
        switch(resourceType){
            default:
                if(GameObject.FindGameObjectsWithTag(resourceType)!=null){
                    resources = GameObject.FindGameObjectsWithTag(resourceType);
                    foreach (GameObject res in resources){
                        resourceDist =
                            Vector3
                                .Distance(transform.position,
                                res.transform.position);
                        if (resourceDist < nearestResourceDist)
                        {
                            nearestResourceDist = resourceDist;
                            nearestResource = res;
                        }
                    }
                    if ((resources != null) && (resources.Length > 0))
                    {
                        System.Array.Clear(resources, 0, resources.Length);
                    }
                }
                break;
            case "cover":
                /*
                foreach (var hitCollider in hitColliders)
                {
                    if(hitCollider.gameObject.CompareTag(resourceType)){
                        nearObjs.Add(hitCollider.gameObject);
                    }
                }*/
                if(GameObject.FindGameObjectsWithTag(resourceType)!=null){
                    resources = GameObject.FindGameObjectsWithTag(resourceType);
                    foreach (GameObject res in resources){
                        resourceDist =
                            Vector3
                                .Distance(transform.position,
                                res.transform.position);
                        if (resourceDist < nearestResourceDist)
                        {
                            nearestResourceDist = resourceDist;
                            nearestResource = res;
                        }
                    } 
                }
                if(nearestResource==null){
                    nearestResource=this.gameObject;
                }
                break;
            case "enemy":
                nearEnemyCount=0;
                for (int i = 0; i < enemyTags.Count; i++){
                    resources = GameObject.FindGameObjectsWithTag(enemyTags[i]);
                    foreach (GameObject res in resources){
                        resourceDist =
                            Vector3
                                .Distance(transform.position,
                                res.transform.position);
                        if (resourceDist < nearestResourceDist)
                        {
                            nearestResourceDist = resourceDist;
                            nearestResource = res;
                        }
                        if(resourceDist<shootDist){
                            nearEnemyCount+=1;
                        }
                    }
                    if ((resources != null) && (resources.Length > 0))
                    {
                        System.Array.Clear(resources, 0, resources.Length);
                    }
                }
                if(nearestResource!=null){
                    if(nearestResource.GetComponent<NewNpcScr>()!=null){
                        UpdateRecentEnemies(nearestResource,nearestResource.GetComponent<NewNpcScr>().threatScore);
                    }else{
                        UpdateRecentEnemies(nearestResource,100f);
                    }
                    if((recentEnemies!=null)&&(recentEnemies.Count>0)){
                        nearestEnemy=recentEnemyScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                    }
                }
                break;
            case "ally":
                nearAllyCount=0;
                for (int i = 0; i < allyTags.Count; i++){
                    resources = GameObject.FindGameObjectsWithTag(allyTags[i]);
                    foreach (GameObject res in resources){
                        resourceDist =
                            Vector3
                                .Distance(transform.position,
                                res.transform.position);
                        if (resourceDist < nearestResourceDist)
                        {
                            if(resourceDist>1f){
                                nearestResourceDist = resourceDist;
                                nearestResource = res;
                            }
                            
                        }
                        if(resourceDist<shootDist){
                            nearAllyCount+=1;
                        }
                    }
                    if ((resources != null) && (resources.Length > 0))
                    {
                        System.Array.Clear(resources, 0, resources.Length);
                    }
                }
                
            break;
        }
        if(nearestResource!=null){
            nearestResourcePos=nearestResource.transform.position;
        }
        return nearestResource;
     }

     private void SetBodyParts(){
        headIntact=true;
        armLeftIntact=true;
        armRightIntact=true;
        legLeftIntact=true;
        legRightIntact=true;
        chestIntact=true;
        if(head==null){
            if(controlScr.RecursiveFindChild(transform,"head")!=null){
                head=controlScr.RecursiveFindChild(transform,"head").gameObject;
            }
        }
        if(head==null){
            if(controlScr.RecursiveFindChild(transform,"neck")!=null){
                head=controlScr.RecursiveFindChild(transform,"neck").gameObject;
            }
        }//Use neck if head not found
        if(chest==null){
            if(controlScr.RecursiveFindChild(transform,"chest")!=null){
                chest=controlScr.RecursiveFindChild(transform,"chest").gameObject;
            }else{
                if(controlScr.RecursiveFindChild(transform,"spine_03")!=null){
                    chest=controlScr.RecursiveFindChild(transform,"spine_03").gameObject;
                }
            }
        }
        if(armLeft==null){
            if(controlScr.RecursiveFindChild(transform,"upperarm_l")!=null){
                armLeft=controlScr.RecursiveFindChild(transform,"upperarm_l").gameObject;
            }
        }
        if(armLeft==null){
            if(controlScr.RecursiveFindChild(transform,"upper_arm_L")!=null){
                armLeft=controlScr.RecursiveFindChild(transform,"upper_arm_L").gameObject;
            }
        }
        if(armRight==null){
            if(controlScr.RecursiveFindChild(transform,"upperarm_r")!=null){
                armRight=controlScr.RecursiveFindChild(transform,"upperarm_r").gameObject;
            }
        }
        if(armRight==null){
            if(controlScr.RecursiveFindChild(transform,"upper_arm_R")!=null){
                armRight=controlScr.RecursiveFindChild(transform,"upper_arm_R").gameObject;
            }
        }
        if(legLeft==null){
            if(controlScr.RecursiveFindChild(transform,"thigh_l")!=null){
                legLeft=controlScr.RecursiveFindChild(transform,"thigh_l").gameObject;
            }
        }
        if(legLeft==null){
            if(controlScr.RecursiveFindChild(transform,"thigh_L")!=null){
                legLeft=controlScr.RecursiveFindChild(transform,"thigh_L").gameObject;
            }
        }
        if(legRight==null){
            if(controlScr.RecursiveFindChild(transform,"thigh_r")!=null){
                legRight=controlScr.RecursiveFindChild(transform,"thigh_r").gameObject;
            }
        }
        if(legRight==null){
            if(controlScr.RecursiveFindChild(transform,"thigh_R")!=null){
                legRight=controlScr.RecursiveFindChild(transform,"thigh_R").gameObject;
            }
        }
        if (head != null){
            if(head.GetComponent<bodypartScr>()==null){
                head.AddComponent<bodypartScr>();
                head.GetComponent<bodypartScr>().body=this.gameObject;
                head.GetComponent<bodypartScr>().type="head";
            }
            headHp=40f;
            head.GetComponent<bodypartScr>().hp = headHp;
        }
        if (chest != null){
            if(chest.GetComponent<bodypartScr>()==null){
                chest.AddComponent<bodypartScr>();
                chest.GetComponent<bodypartScr>().body=this.gameObject;
                chest.GetComponent<bodypartScr>().type="chest";
            }
        }
        if (armLeft != null){
            if(armLeft.GetComponent<bodypartScr>()==null){
                armLeft.AddComponent<bodypartScr>();
                armLeft.GetComponent<bodypartScr>().body=this.gameObject;
                armLeft.GetComponent<bodypartScr>().type="armLeft";
            }
            armLeftHp=20f;
            armLeft.GetComponent<bodypartScr>().hp = armLeftHp;
        }
        if (armRight != null){
            if(armRight.GetComponent<bodypartScr>()==null){
                armRight.AddComponent<bodypartScr>();
                armRight.GetComponent<bodypartScr>().body=this.gameObject;
                armRight.GetComponent<bodypartScr>().type="armRight";
            }
            armRightHp=20f;
            armRight.GetComponent<bodypartScr>().hp = armRightHp;
        }
        if (legLeft != null){
            if(legLeft.GetComponent<bodypartScr>()==null){
                legLeft.AddComponent<bodypartScr>();
                legLeft.GetComponent<bodypartScr>().body=this.gameObject;
                legLeft.GetComponent<bodypartScr>().type="legLeft";
            }
            legLeftHp=20f;
            legLeft.GetComponent<bodypartScr>().hp = legLeftHp;
        }
        if (legRight != null){
            if(legRight.GetComponent<bodypartScr>()==null){
                legRight.AddComponent<bodypartScr>();
                legRight.GetComponent<bodypartScr>().body=this.gameObject;
                legRight.GetComponent<bodypartScr>().type="legRight";
            }
            legRightHp=20f;
            legRight.GetComponent<bodypartScr>().hp = legRightHp;
        }
    }

    public void SetOutfit(string oft="",string oftHead="",string oftChest="",
    string oftArmLeft="",string oftArmRight="",string oftLegLeft="",string oftLegRight=""){
        string oTxt="Outfits\\";
        GameObject tmpHead=null,
        tmpArmLeft=null,
        tmpArmRight=null,
        tmpLegLeft=null,
        tmpLegRight=null,
        tmpChest=null;
        if(string.IsNullOrEmpty(oft)){
            oft="default";
        }
        if(outfitHead!=null){
            Destroy(outfitHead.gameObject);
        }
        if(outfitArmLeft!=null){
            Destroy(outfitArmLeft.gameObject);
        }
        if(outfitArmRight!=null){
            Destroy(outfitArmRight.gameObject);
        }
        if(outfitLegLeft!=null){
            Destroy(outfitLegLeft.gameObject);
        }
        if(outfitLegRight!=null){
            Destroy(outfitLegRight.gameObject);
        }
        if(outfitChest!=null){
            Destroy(outfitChest.gameObject);
        }
        switch(oft){
            case "none":

                break;
            default:
            oft="default";
                if(head!=null){
                    //tmpHead=controlScr.SpawnObj(oTxt+oft+"Head",head.transform.position,head.transform.rotation,false);
                }
                if(armLeft!=null){
                    tmpArmLeft=controlScr.SpawnObj(oTxt+oft+"ArmLeft",armLeft.transform.position,armLeft.transform.rotation,false);
                }
                if(armRight!=null){
                    tmpArmRight=controlScr.SpawnObj(oTxt+oft+"ArmRight",armRight.transform.position,armRight.transform.rotation,false);
                }
                if(legLeft!=null){
                    tmpLegLeft=controlScr.SpawnObj(oTxt+oft+"LegLeft",legLeft.transform.position,legLeft.transform.rotation,false);
                }
                if(legRight!=null){
                    tmpLegRight=controlScr.SpawnObj(oTxt+oft+"LegRight",legRight.transform.position,legRight.transform.rotation,false);
                }
                if(chest!=null){
                    tmpChest=controlScr.SpawnObj(oTxt+oft+"Chest",chest.transform.position,chest.transform.rotation,false);
                }
                break;
        }
        if(!String.IsNullOrEmpty(oftHead)){
            if(head!=null){
                if(tmpHead!=null){
                    Destroy(tmpHead);
                }
                tmpHead=controlScr.SpawnObj(oTxt+oftHead+"Head",head.transform.position,head.transform.rotation,false);
            }
        }
        if(!String.IsNullOrEmpty(oftChest)){
            if(chest!=null){
                if(tmpChest!=null){
                    Destroy(tmpChest);
                }
                tmpChest=controlScr.SpawnObj(oTxt+oftChest+"Chest",chest.transform.position,chest.transform.rotation,false);
            }
        }
        if(!String.IsNullOrEmpty(oftArmLeft)){
            if(armLeft!=null){
                if(tmpArmLeft!=null){
                    Destroy(tmpArmLeft);
                }
                tmpArmLeft=controlScr.SpawnObj(oTxt+oftArmLeft+"ArmLeft",armLeft.transform.position,armLeft.transform.rotation,false);
            }
        }
        if(!String.IsNullOrEmpty(oftArmRight)){
            if(armRight!=null){
                if(tmpArmRight!=null){
                    Destroy(tmpArmRight);
                }
                tmpArmRight=controlScr.SpawnObj(oTxt+oftArmRight+"ArmRight",armRight.transform.position,armRight.transform.rotation,false);
            }
        }
        if(!String.IsNullOrEmpty(oftLegLeft)){
            if(legLeft!=null){
                if(tmpLegLeft!=null){
                    Destroy(tmpLegLeft);
                }
                tmpLegLeft=controlScr.SpawnObj(oTxt+oftLegLeft+"LegLeft",legLeft.transform.position,legLeft.transform.rotation,false);
            }
        }
        if(!String.IsNullOrEmpty(oftLegRight)){
            if(legRight!=null){
                if(tmpLegRight!=null){
                    Destroy(tmpLegRight);
                }
                tmpLegRight=controlScr.SpawnObj(oTxt+oftLegRight+"LegRight",legRight.transform.position,legRight.transform.rotation,false);
            }
        }
        if ((head != null)&&(headIntact)){
            if(tmpHead!=null){
                outfitHead=tmpHead;
                outfitHead.transform.SetParent(head.transform, true);
            }
        }
        if ((armLeft != null)&&(armLeftIntact)){
            if(tmpArmLeft!=null){
                outfitArmLeft=tmpArmLeft;
                outfitArmLeft.transform.SetParent(armLeft.transform, true);
            }
        }
        if ((armRight != null)&&(armRightIntact)){
            if(tmpArmRight!=null){
                outfitArmRight=tmpArmRight;
                outfitArmRight.transform.SetParent(armRight.transform, true);
            }
        }
        if ((legLeft != null)&&(legLeftIntact)){
            if(tmpLegLeft!=null){
                outfitLegLeft=tmpLegLeft;
                outfitLegLeft.transform.SetParent(legLeft.transform, true);
            }
        }
        if ((legRight != null)&&(legRightIntact)){
            if(tmpLegRight!=null){
                outfitLegRight=tmpLegRight;
                outfitLegRight.transform.SetParent(legRight.transform, true);
            }
        }
        if ((chest != null)&&(chestIntact)){
            if(tmpChest!=null){
                outfitChest=tmpChest;
                outfitChest.transform.SetParent(chest.transform, true);
            }
        }
     }

    public void Gore(){
        if ((head != null) && (headIntact)){
            if (headHp < 0)
            {
                head.transform.localScale -= new Vector3(1f, 1f, 1f);
                goreCount += 1;
                hp = 0f;
                headIntact = false;
                switch(bloodType){
                    default:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObj,head.transform.position,transform.rotation);
                        }
                        break;
                    case 1:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObjZerran,head.transform.position,transform.rotation);
                        }
                        break;
                }
            }
        }
        if ((armLeft != null) && (armLeftIntact)){
            if (armLeftHp < 0)
            {
                armLeft.transform.localScale -= new Vector3(1f, 1f, 1f);
                goreCount += 1;
                hp -= 10f;
                armLeftIntact = false;
                switch(bloodType){
                    default:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObj,armLeft.transform.position,transform.rotation);
                        }
                        break;
                    case 1:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObjZerran,armLeft.transform.position,transform.rotation);
                        }
                        break;
                }
            }
        }
        if ((armRight != null) && (armRightIntact)){
            if (armRightHp < 0)
            {
                armRight.transform.localScale -= new Vector3(1f, 1f, 1f);
                goreCount += 1;
                hp -= 10f;
                armRightIntact = false;
                switch(bloodType){
                    default:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObj,armRight.transform.position,transform.rotation);
                        }
                        break;
                    case 1:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObjZerran,armRight.transform.position,transform.rotation);
                        }
                        break;
                }
                if(curWep<100){
                    controlScr.SpawnObj("wep",new Vector3(transform.position.x, 
                    transform.position.y + 2f, 
                    transform.position.z),transform.rotation,true,curWep);
                    SetWeapon(100);
                    wepsObj.SetActive(false);
                }
            }
        }
        if ((legLeft != null) && (legLeftIntact)){
            if (legLeftHp < 0)
            {
                legLeft.transform.localScale -= new Vector3(1f, 1f, 1f);
                goreCount += 1;
                hp -= 10f;
                legLeftIntact = false;
                moveSpeed/=4f;
                switch(bloodType){
                    default:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObj,legLeft.transform.position,transform.rotation);
                        }
                        break;
                    case 1:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObjZerran,legLeft.transform.position,transform.rotation);
                        }
                        break;
                }
            }
        }
        if ((legRight != null) && (legRightIntact)){
            if (legRightHp < 0)
            {
                legRight.transform.localScale -= new Vector3(1f, 1f, 1f);
                goreCount += 1;
                hp -= 10f;
                legRightIntact = false;
                moveSpeed/=4f;
                switch(bloodType){
                    default:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObj,legRight.transform.position,transform.rotation);
                        }
                        break;
                    case 1:
                        if (controlScr.bloodObj != null){
                            Instantiate(controlScr.bloodObjZerran,legRight.transform.position,transform.rotation);
                        }
                        break;
                }
            }
        }
        if((!legLeftIntact) && (!legRightIntact)){
            canMove=false;
            moveSpeed=0;
            moveDir.x=0;
            moveDir.z=0;
            if(controllerActive){
                controller.height/=2f;
                if(capCol!=null){
                    capCol.height/=2f;
                }
            }
            switch(bloodType){
                default:
                    if (controlScr.bloodObj != null){
                        Instantiate(controlScr.bloodObj,transform.position,transform.rotation);
                    }
                    break;
                case 1:
                    if (controlScr.bloodObj != null){
                        Instantiate(controlScr.bloodObjZerran,transform.position,transform.rotation);
                    }
                    break;
            }
        }
        if (goreCount > 3){
            if (chestIntact){
                if(chest != null){
                    chest.transform.localScale -= new Vector3(1f, 1f, 1f);
                }
                chestIntact = false;
            }
            if (controlScr.bodyPartsObj != null){
                Instantiate(controlScr.bodyPartsObj,
                transform.position,
                transform.rotation);
            }
            switch(bloodType){
                default:
                    if (controlScr.bloodObj != null){
                        Instantiate(controlScr.bloodObj,transform.position,transform.rotation);
                    }
                    break;
                case 1:
                    if (controlScr.bloodObj != null){
                        Instantiate(controlScr.bloodObjZerran,transform.position,transform.rotation);
                    }
                    break;
            }
            hp = 0f;
        }
    }

    void Crouch(bool cr){
        if(controller!=null){
            isCrouched=cr;
            if(cr){
                /*
                controller.height=crouchHeight;
                if(capCol!=null){
                    //capCol.height=crouchHeight;
                }
                */
                //Crouch anim
                if (myAnimation != null)
                {
                    foreach (AnimationState state in myAnimation)
                    {
                        if (state.name == "crouch")
                        {
                            PlayAnim("crouch");
                        }
                    }
                }
            }
            else{
                /*
                controller.height=regHeight;
                if(capCol!=null){
                    capCol.height=regHeight;
                }*/
            }
        }
    }

    void UseAbility(string abil)
    {
        switch (abil)
        {
            default:
                abilityObj = controlScr.SpawnObj(abil, shootPos, transform.rotation);
                abilityObj.transform.LookAt(lastEnemyPos);
                abilityObj.GetComponent<abilityScr>().owner = this.gameObject;
                abilityObj.GetComponent<abilityScr>().team = objTeam;
                break;

        }
    }

    void Shoot(){
        if((nearestEnemy==playerObj)&&(!playerScr.isAlive)){
            //taunt dead player
            Crouch(!isCrouched);
        }else{
            switch (curWep){
                default:
                case 0://Rifle
                    CreateBullet(0.02f,0f,0.02f);
                    break;
                case 1://Pistol
                    CreateBullet(0f,0.03f,0f);
                    break;
                case 2://Shotgun
                    CreateBullet(0.3f,0.3f,0.3f);
                    CreateBullet(0.3f,0.3f,0.3f);
                    CreateBullet(0.3f,0.3f,0.3f);
                    CreateBullet(0.3f,0.3f,0.3f);
                    break;
                case 3://Void
                    CreateBullet();
                    break;
                case 4://Lancer
                    CreateBullet(0.1f,0.1f,0.1f);
                    CreateBullet(0.1f,0.1f,0.1f);
                    break;
                case 5://Sniper
                    CreateBullet();
                    break;
                case 6://Rail
                    CreateBullet(0.01f,0.2f,0.01f);
                    break;
                case 7://Razor
                    CreateBullet(0.05f,0f,0.05f);
                    break;
                case 8://Handcannon
                    CreateBullet(0.1f,0.1f,0.1f);
                    break;
                case 101://Tank
                    CreateBullet(0.05f,0.1f,0.05f);
                    break;
                case 102://Hovercraft
                    CreateBullet(0.05f,0.1f,0.05f);
                    break;
                case 103://Carrier
                    CreateBullet(0.2f,0.4f,0.2f);
                    break;
                case 100:
                    break;
            }
            if(isMoving){
                navTimer+=fireAimTimer;
                moveHoldTimer+=fireAimTimer;
            }else{
                Crouch(true);
            }
            sounds.clip=fireSound;
            sounds.Play();
            ammo-=1;
        }
     }

     void CreateBullet(float rvx=0f,float rvy=0f,float rvz=0f){//determine accuracy
        curBul =Instantiate(controlScr.bulletObj,shootPos + (transform.forward),transform.rotation);
        if(isFlying){
            ranTarget=lastEnemyPos;
        }else{
            ranTarget = new Vector3(lastEnemyPos.x + UnityEngine.Random.Range(-rvx, rvx),
            lastEnemyPos.y + UnityEngine.Random.Range(-rvy, rvy),
            lastEnemyPos.z + UnityEngine.Random.Range(-rvz, rvz));
        }
        bulScr = curBul.GetComponent<BulScr>();
        bulScr.objname = this.gameObject;
        bulScr.team = objTeam;
        bulScr.type = curWep;
        bulScr.dmg = dmg;
        bulScr.targetPos=ranTarget;
        bulScr.fireSpeed=fireSpeed;
        if(nearestEnemy!=null){
            bulScr.trackTarget=nearestEnemy;
        }
     }

     void Melee(){
        curMelee=Instantiate(meleeObj,shootPos+transform.forward,transform.rotation);
        melScr=curMelee.GetComponent<meleeScr>();
        melScr.team=objTeam;
        melScr.objTitle=objName;
        melScr.objname=this.gameObject;
        melScr.dmg=meleeDmg;
        meleeTimer=0.5f;
        curMelee.transform.LookAt(aimTarget=new Vector3(UnityEngine.Random.Range(-180,180),UnityEngine.Random.Range(-180,180),UnityEngine.Random.Range(-180,180)));
        curMelee.GetComponent<Rigidbody>().linearVelocity =curMelee.transform.forward * 25f;
        if(myAnimation!=null){
            foreach (AnimationState state in myAnimation)
            {
                if (state.name == "melee")
                {
                    PlayAnim("melee");
                }
            }
        }
     }

     void Grenade(){
        switch (greType){
            default:
            case 0:
                //regular
                curGre=Instantiate(controlScr.grenadeObj,
                    new Vector3(shootPos.x, shootPos.y+shootPosMod, shootPos.z),
                    transform.rotation);
                break;
            case 1:
                //plasma
                curGre=Instantiate(controlScr.plasmaGrenadeObj,
                    new Vector3(shootPos.x, shootPos.y + shootPosMod, shootPos.z),
                    transform.rotation);
                break;
        }
        greCount -= 1;
        greScr=curGre.GetComponent<grenadeScr>();
        greScr.owner=this.gameObject;
        greScr.team=objTeam;
        curGre.transform.LookAt(new Vector3(lastEnemyPos.x, lastEnemyPos.y+ (nearestEnemyDist/2f), lastEnemyPos.z));
        curGre.GetComponent<Rigidbody>().AddForce(curGre.transform.forward *(150f+nearestEnemyDist));
        TalkSounds("grenadeSounds");
     }

     void Rocket(){
        //curGre=
     }

     void DrawWeapon(bool dw){
        if(wepsObj!=null){
            wepsObj.SetActive(dw);
        }
     }

     void SetWeapon(int cw){
        if(cw!=100){
            dmg=controlScr.weaponsDamage[cw];
            fireSpeed=controlScr.weaponsFirespeed[cw];
            ammoMax=controlScr.weaponsAmmo[cw];
            ammo=ammoMax;
            reloadTime=controlScr.weaponsReloadtime[cw];
            fireSound=controlScr.weaponsSounds[cw];
            attackMin=controlScr.weaponsAttacktime[cw];
            attackMax=attackMin*3f;
            DrawWeapon(true);
        }
        if(wepsObj!=null){
            foreach (Transform eachChild in wepsObj.transform) {
                if (eachChild.name == ("wep"+cw.ToString())) {
                    eachChild.gameObject.SetActive(true);
                }else{
                    eachChild.gameObject.SetActive(false);
                }
            }
        }
        curWep=cw;
     }

     void SwapWep(int sw){//canSwapWep means it can swap at all, canSwap and swapTimer are time limiter
        if((canSwapWep)&&(canSwap)){
            canSwap=false;
            swapTimer=10f;
            tempWep=curWep;
            tempString="wep";
            if(tempWep<100){
                controlScr.SpawnObj(tempString,transform.position,transform.rotation,true,tempWep);
            }
            curWep=sw;
            SetWeapon(curWep);
        }
     }

     void Recharge(){
        shields+=(maxShields/100f);
        if(shields>maxShields){
            shields=maxShields;
        }
     }

     IEnumerator Reload(){
        Navigate("cover");
        yield return new WaitForSeconds(reloadTime);
        ammo=ammoMax;
        canShoot=true;
        //talk reload
     }

     private void PlayFootStepAudio(){
        if (!controller.isGrounded)
        {
            return;
        }
        footstepSounds.clip = stepSounds[UnityEngine.Random.Range(0, stepSounds.Count-1)];
        footstepSounds.PlayOneShot(footstepSounds.clip);
        footstepTimer = moveSpeed / 20f;
        
    }

     public void Hit(GameObject atkr,float dg,string dgType,Vector3 hitPos){//Attacker, damage, type, where
        rechargeTimer=rechargeRate;//How long before shields recharge
        Alert();//Set alert, inBattle and timer to 60 seconds
        impulseTimer+=0.1f*dg;
        if(!holdingFlag){
            if(impulseTimer>5f){
                DodgeRelativePosition(hitPos);
            }
        }
        if(atkr!=null){
            recentEnemy=atkr;
            if(recentEnemy.GetComponent<NewNpcScr>()!=null){
                recentEnemy.GetComponent<NewNpcScr>().hitScore+=dg;
            }
            UpdateRecentEnemies(recentEnemy,1f);
            navControls["combat"]+=0.1f;
            navGoals["hunt"]+=0.1f;
            if(hp<(maxHp/2f)){
                if(isLeader){
                    navGoals["hide"]+=1f;
                    navGoals["survive"]+=1f;
                    navPlans["pursue"]+=1f;
                }else{
                    navGoals["hide"]+=2f;
                    navGoals["survive"]+=1f;
                    if(hasLeader){
                        CheckRelativePosition();
                    }
                }
                
            }
            //make enemy react if you hit them directly from far away
            if(Vector3.Distance(transform.position,atkr.transform.position)>shootDist){
                shootDist=Vector3.Distance(transform.position,atkr.transform.position);
            }
            sightAngle=Vector3.Angle(hitPos-shootPos,transform.forward);
            if((sightAngle>-145f)&&(sightAngle<145f)){
                lastEnemyPos=atkr.transform.position;
                nearestEnemy=atkr;
                if(atkr.CompareTag("Player")){
                    playerScr.hitAimTimer=0.1f;
                    if(reputation>0){
                        ChangeRep(0,false);
                        friendlyFire=true;
                        if(hp>(maxHp/2)){
                            forgiveTimer=1.5f;
                            aimTargetOverrideTimer+=1.5f;
                            aimTargetOverride=lastEnemyPos;
                        }else{
                            forgiveTimer=3f;
                            aimTargetOverrideTimer+=3f;
                            aimTargetOverride=lastEnemyPos;
                        }
                    }
                }
            }
            lastEnemyPos = atkr.transform.position;
            if (atkr.GetComponent<NewNpcScr>() != null){
                if(hp <= 0){
                    atkr.GetComponent<NewNpcScr>().Kill();
                    atkr.GetComponent<NewNpcScr>().UpdateRecentEnemies(this.gameObject,0f);
                }
            }
        }
        deathType=dgType;//Override deathType based on current hit type
        //If dgType is weakness, amplify dmg. If strength, lessen dmg.
        switch(dgType){
            default:
            case "shot":
                if (shields <= 0)
                {
                    TalkSounds("hitSounds", 5);
                }
                else
                {
                    TalkSounds("shieldHitSounds", 0);
                }
                if(nearestEnemyDist>(shootDist/2f)){
                    switch(curWep){
                        default:
                            navGoals["findweaponranged"]+=1f;
                            break;
                        case 5:
                        case 101:
                        case 102:
                        case 103:
                            if(canSeeEnemy){
                                navGoals["hunt"]+=1f;
                            }else{
                                navGoals["findenemy"]+=1f;
                            }
                            break;
                    }
                }else if(nearestEnemyDist>(shootDist/4f)){

                }else{
                    switch(curWep){
                        default:
                            navGoals["findweaponclose"]+=1f;
                            break;
                        case 2:
                        case 101:
                        case 102:
                        case 103:
                            break;
                    }
                }
                break;
            case "melee":
                TalkSounds("meleeSounds");
                if(canBleed){
                    sounds.clip = controlScr.hitSoundFlesh1;
                    sounds.Play();
                }
                goreCount+=3;
                Gore();
                hitAngle = Vector3.Angle(hitPos-transform.position,transform.forward);
                switch(objName){
                    default:
                        if ((hitAngle > -45f) && (hitAngle < 45f)){
                            if(!inBattle){
                                dg*=2f;
                            }
                        }
                        attackTimer+=0.5f;
                        aimTargetOverrideTimer+=0.5f;
                        aimTargetOverride=hitPos-transform.position;
                        PlayAnim("hitHead",true,1f);
                        break;
                    case "dominionTank":
                    case "coalitionTank":
                        dg=0f;
                        break;
                }
                
                break;
            case "matrix":
            case "explosion":
                navGoals["survive"]+=5f;
                break;
        }
        if(shields>0f){
            if (shields > dg)
            {
                shields -= dg;
                dg = 0;
            }
            else
            {
                shields -= dg;
                dg -= (dg-(shields+dg));
            }
            //Subtract
        }
        if(shields<=0f){//Call this again because you want to damage once shields are down, too
            hp-=dg;
            if(canBleed){
                switch(bloodType){
                    default:
                    case 0://Human blood
                        Instantiate(controlScr.bloodObj,hitPos,transform.rotation);
                        break;
                    case 1://Zerran blood
                        Instantiate(controlScr.bloodObjZerran,hitPos,transform.rotation);
                        break;
                }
            }
        }
        Gore();
        if(!navActive){
            ChangeNavSettings("hide");
        }
     }

     public void ChangeNavSettings(string ct="",string nt="",float msp=-1f,float nmin=-1f,float rst=60f){
        if(!string.IsNullOrEmpty(ct)){
            controlNavType=ct;
            navActive=true;
            navTimer=1f;
            if(GameObject.Find(controlNavType)!=null){
                navTargetObj=GameObject.Find(controlNavType);
                navTargetObjDist=Vector3.Distance(transform.position,navTargetObj.transform.position);
            }
            switch(controlNavType){
                default:
                    manualPathOverride=true;
                    break;
                case "none":
                case "hold":
                case "tagalong":
                    break;
            }
        }
        if(!string.IsNullOrEmpty(nt)){
            SetNavType(nt);
        }
        if(msp>=0f){
            moveSpeed=msp;
        }
        if(!navActive){
            navActive=true;
            navTimer=1f;
            navResetTimer=rst;
            navResetActive=true;
            if(talkCollider!=null){
                switch(objName){
                    default:
                        talkCollider.radius=10;
                        break;
                    case "AmalMoore":
                        talkCollider.radius=10;
                        break;
                }
            }
        }
        if(nmin>=0f){
            navMin=nmin;
            navMax=navMin*2f;
        }
     }

     public void AllyDeath(bool isl=false){
        if(isLeader){
            navGoals["cleararea"]+=1f;
            navGoals["hunt"]+=1f;
        }else if(isFollower){
            if(isl){
                navGoals["hide"]+=1f;
                navGoals["survive"]+=1f;
            }else{
                navGoals["group"]+=1f;
                navGoals["cleararea"]+=1f;
            }
        }else{
            navGoals["cleararea"]+=1f;
        }
        if(nearAllyCount>0){
            navGoals["group"]+=1f;
        }
        if(nearAllyCount>nearEnemyCount){
            navGoals["hunt"]+=1f;
            navGoals["cleararea"]+=1f;
        }else{
            navGoals["survive"]+=1f;
            navGoals["hide"]+=1f;
        }
     }

     public void AllyKill(){
        if(isLeader){
            navGoals["hunt"]+=2f;
        }else if(isFollower){
            navGoals["hunt"]+=1f;
        }else{
            navGoals["hunt"]+=1f;
        }
     }

     void Death(){
        isActive=false;
        SetCount(-1);
        if(curWep<100){
            controlScr.SpawnObj("wep",new Vector3(transform.position.x, 
            transform.position.y + 2f+shootPosMod, 
            transform.position.z),transform.rotation,true,curWep);
        }
        if (greCount > 0){
            while (greCount > 0){
                greCount -= 1;
                switch(greType){
                    default:
                    controlScr.SpawnObj("wep99",new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z),transform.rotation,false);
                    break;
                    case 1:
                    controlScr.SpawnObj("wep98",new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z),transform.rotation,false);
                    break;
                }
                
            }
        }
        if (healObjCount > 0){
            while (healObjCount > 0){
                healObjCount -= 1;
                controlScr.SpawnObj("healObj",transform.position,transform.rotation,false);
            }
        }
        if(hasLeader){
            if(leaderObj!=null){
                leaderObj.GetComponent<NewNpcScr>().UpdateSquad(this.gameObject,false);
            }
        }
        if(nearestAlly!=null){
            if(nearestAlly.GetComponent<NewNpcScr>()!=null){
                if(isLeader&&hasSquad){
                    nearestAlly.GetComponent<NewNpcScr>().AllyDeath(true);
                }else{
                    nearestAlly.GetComponent<NewNpcScr>().AllyDeath();
                }
            }
        }
        if(deadObj!=null){
            tempDead=Instantiate(deadObj,transform.position,transform.rotation);
            if(tempDead.GetComponent<deadScr>()!=null){
                deathScr=tempDead.GetComponent<deadScr>();
            }else{
                tempDead.gameObject.AddComponent<deadScr>();
                deathScr=tempDead.GetComponent<deadScr>();
            }
            if(deathScr!=null){
                deathScr.headIntact=headIntact;
                deathScr.legLeftIntact=legLeftIntact;
                deathScr.legRightIntact=legRightIntact;
                deathScr.armLeftIntact=armLeftIntact;
                deathScr.armRightIntact=armRightIntact;
                deathScr.chestIntact=chestIntact;
                deathScr.Gore();
                if(inventoryList!=null){//spawn any inventory items
                    if(inventoryList.Count>0){
                        foreach(string invItem in inventoryList){
                            if(invItem!=null){
                                deathScr.inventoryList.Add(invItem);
                            }
                        }
                    }
                }
            }else{
                if(inventoryList!=null){//spawn any inventory items
                    if(inventoryList.Count>0){
                        foreach(string invItem in inventoryList){
                            if(invItem!=null){
                                controlScr.SpawnObj(invItem, new Vector3(transform.position.x+UnityEngine.Random.Range(-1,1f),
                                    transform.position.y+1f,transform.position.z+UnityEngine.Random.Range(-1,1f)),Quaternion.identity,false);
                            }
                        }
                    }
                }
            }
        }else{
            if(inventoryList!=null){//spawn any inventory items
                if(inventoryList.Count>0){
                    foreach(string invItem in inventoryList){
                        if(invItem!=null){
                            controlScr.SpawnObj(invItem, new Vector3(transform.position.x+UnityEngine.Random.Range(-1,1f),
                                transform.position.y+1f,transform.position.z+UnityEngine.Random.Range(-1,1f)),Quaternion.identity,false);
                        }
                    }
                }
            }
        }
        if (recentEnemy!=null){
            if (recentEnemy.CompareTag("Player")){
                controlScr.killCount+=1;//record player kill
                controlScr.AddExperience(expAmount);
                if(reputation>0){
                    witnessed =CheckWitnesses("player");
                    if (witnessed)
                    {
                        ChangeRep(0);//change reputation
                    }
                }else{
                    ChangeRep(reputation-1);
                }
            }else{
                if(!inBattle){
                    witnessed =CheckWitnesses("alert");
                }
            }
            
        }
        if(importantCharacter){
            controlScr.ShowMessage(objName+" has died.","normal",3f);//show message they died
            controlScr.UpdateMission(objName,"dead");//update characters mission if it exists
        }
        //Character deaths that end missions
        switch(objName){
            default:
                break;
            case "BanditLeader":
                foreach(GameObject ban in GameObject.FindGameObjectsWithTag("bandit")){
                    if(Vector3.Distance(GameObject.Find("BanditLeader").transform.position,ban.transform.position)<500f){
                        ban.GetComponent<NewNpcScr>().ChangeRep(0,false);
                        ban.GetComponent<NewNpcScr>().ChangeNavSettings("none","");
                    }
                }
                switch(controlScr.missionStatus["Collective1"]){
                    default:
                        controlScr.UpdateMission("Collective1","ambushanonymous","","Nahlia wants to speak to you about the bandits.","NahliaJericho","Bugged Out","show");
                        controlScr.UpdateMission("NahliaJericho","ambushanonymous","startambushanonymous");
                        break;
                    case "meetuptruth":
                        controlScr.UpdateMission("Collective1","ambushwin","","Inform Nahlia the bandits have been taken care of.","NahliaJericho","Bugged Out","show");
                        controlScr.UpdateMission("NahliaJericho","ambushwin","startambushwin");
                        if(GameObject.Find("JahRook")!=null){
                            GameObject.Find("JahRook").GetComponent<NewNpcScr>().objTeam="bandit";
                        }
                        controlScr.SpawnObj("rebelSoldier",GameObject.Find("BanditAmbushSpawn1").transform.position,Quaternion.identity);
                        controlScr.SpawnObj("rebelSoldier",GameObject.Find("BanditAmbushSpawn2").transform.position,Quaternion.identity);
                        controlScr.SpawnObj("rebelSoldier",GameObject.Find("BanditAmbushSpawn3").transform.position,Quaternion.identity);
                        controlScr.SpawnObj("rebelSoldier",GameObject.Find("BanditAmbushSpawn4").transform.position,Quaternion.identity);
                        break;
                }
                break;
            case "YarahZen":
                controlScr.UpdateMission("JormadFollow","suspect","","","","","hide");
                switch(controlScr.missionStatus["SearchJormad"]){
                    default:
                        controlScr.UpdateMission("SearchJormad","dead","","","","","hide");
                        break;
                    case "found":
                    case "foundhurt":
                        controlScr.UpdateMission("SearchJormad","deadfound","","","","","hide");
                        break;
                    case "foundfree":
                        controlScr.UpdateMission("SearchJormad","deadfree","","","","","hide");
                        break;
                }
                break;
            case "RillBarren":
                if(controlScr.missionStatus["DelarusAttackStatus"]=="start"){
                    controlScr.CallEvent("dominionDelarusAttack");
                }
                break;
        }
        switch(controlScr.gameMode){
            default:
                break;
            case "Deathmatch":
                switch(objTeam){
                    default:

                        break;
                    case "coalition":
                        controlScr.scoreDominion+=1;
                        break;
                    case "dominion":
                        controlScr.scoreCoalition+=1;
                        break;
                }
                break;
            case "Capture the flag":

                break;
        }
        
        Destroy(this.gameObject);
     }

    bool CheckWitnesses(string witnessType="default")
    {
        witnessed = false;
        switch(witnessType){
            default:

            break;
            case "alert":
                for (int i = 0; i < allyTags.Count; i++)
                {
                    foreach (GameObject ally in GameObject.FindGameObjectsWithTag(allyTags[i]))
                    {
                        if(Vector3.Distance(ally.transform.position,transform.position)<shootDist){
                            rayAim = Physics.Raycast(new Vector3(ally.transform.position.x,
                                ally.transform.position.y+2f, ally.transform.position.z),
                                (new Vector3(transform.position.x,
                                transform.position.y + 2f, transform.position.z) - 
                                new Vector3(ally.transform.position.x,
                                ally.transform.position.y + 2f, ally.transform.position.z)), out rayAimHit, shootDist);
                            if ((rayAim) && (rayAimHit.transform != null))
                            {
                                if (rayAimHit.transform.gameObject == this.gameObject)
                                {
                                    sightAngle = Vector3.Angle(transform.position -
                                        ally.transform.position, transform.forward);
                                    if ((sightAngle > -135f) && (sightAngle < 135f))
                                    {
                                        if (!witnessed)
                                        {
                                            witnessed = true;
                                            TalkSounds("casualtySounds");
                                        }
                                        if(!ally.GetComponent<NewNpcScr>().inBattle){
                                            ally.GetComponent<NewNpcScr>().Alert();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case "player":
                for (int i = 0; i < allyTags.Count; i++)
                {
                    foreach (GameObject ally in GameObject.FindGameObjectsWithTag(allyTags[i]))
                    {
                        rayAim = Physics.Raycast(new Vector3(ally.transform.position.x,
                            ally.transform.position.y+2f, ally.transform.position.z),
                            (new Vector3(playerObj.transform.position.x,
                            playerObj.transform.position.y + 2f, playerObj.transform.position.z) - 
                            new Vector3(ally.transform.position.x,
                            ally.transform.position.y + 2f, ally.transform.position.z)), out rayAimHit, shootDist);
                        if ((rayAim) && (rayAimHit.transform != null))
                        {
                            if (rayAimHit.transform.gameObject == playerObj)
                            {
                                sightAngle = Vector3.Angle(playerObj.transform.position -
                                    ally.transform.position, transform.forward);
                                if ((sightAngle > -170f) && (sightAngle < 170f))
                                {
                                    if (!witnessed)
                                    {
                                        witnessed = true;
                                        return witnessed;
                                    }
                                }
                            }
                        }
                    }
                }
                break;
        }
        return witnessed;
    }

    void Moving(bool ism){
        if(isMoving!=ism){
            isMoving=ism;
        }
        if(isCrouched){
            if(isMoving){
                Crouch(false);
            }
        }
        if(ism){
            if(myAnimation!=null){
                if(meleeTimer<=0f){
                    if(inBattle){
                        PlayAnim("run");
                    }else{
                        PlayAnim("walk");
                    }
                }
            }else{
                if(myAnimator!=null){
                    if(animOverrideTimer<=0){
                        myAnimator.SetFloat("Speed",moveSpeed);
                    }
                }
            }
        }else{
            if(myAnimation!=null){
                if(meleeTimer<=0f){
                    if(inBattle){
                        PlayAnim("aim");
                    }else{
                        PlayAnim("idle");
                    }
                }
            }else{
                if(myAnimator!=null){
                    if(animOverrideTimer<=0){
                        myAnimator.SetFloat("Speed",0);
                    }
                }
            }
        }  
    }

     void Talk(string tt,bool objSpecific=false,bool soundSpecific=false){
        string ttnew=tt;
        string tttext="";
        int ttchoice=0;
        if(soundsTalk!=null){//verify talk audiosource exists
            if(talkTimer<=0){
                if(soundSpecific){
                    switch(tt){
                        default:
                            
                            break;
                    }
                }else{
                    switch(tt){
                        default:
                            break;
                        case "follow":

                            break;
                        case "help":

                            break;
                        case "allykill":

                            break;
                        case "stopit":
                            if (controlScr.repCoalition > 0)
                                {
                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                    ttnew = tt + ttchoice.ToString();
                                    switch (ttchoice)
                                    {
                                        default:
                                        case 0:
                                            tttext = "Cut it out!";
                                            break;
                                        case 1:
                                            tttext = "Stop it.";
                                            break;
                                        case 2:
                                            tttext = "What do you think you're doing?";
                                            break;
                                    }
                                }
                            break;
                        case "itsok":
                            tttext = "It's ok.";
                            break;
                        case "fine":
                            tttext = "I'm fine. You?";
                            otherTalker.GetComponent<NewNpcScr>().Talk("finetoo");
                            navTimer+=5f;
                            otherTalker.GetComponent<NewNpcScr>().navTimer+=5f;
                            break;
                        case "finetoo":
                            tttext = "I've been better.";
                            break;
                        case "goodtoseeyoutoo":
                            tttext = "Good to see you too.";
                            break;
                        case "banterally":
                            if(!inBattle){
                                navTarget=transform.position;
                                navTimer+=5f;
                                otherTalker.GetComponent<NewNpcScr>().navTarget=otherTalker.transform.position;
                                otherTalker.GetComponent<NewNpcScr>().navTimer+=5f;
                                if(otherTalker.GetComponent<NewNpcScr>().otherTalker!=this.gameObject){
                                    switch (objName)
                                    {
                                        default:
                                            tttext = "Excuse me.";
                                            break;
                                        case "coalitionCivilian":
                                            switch(controlScr.missionStatus["BanterCoalition"]){
                                                default:
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "How's it going?";
                                                            otherTalker.GetComponent<NewNpcScr>().Talk("fine");
                                                            break;
                                                        case 1:
                                                            tttext = "Good to see you.";
                                                            otherTalker.GetComponent<NewNpcScr>().Talk("goodtoseeyoutoo");
                                                            break;
                                                        case 2:
                                                            tttext = "Sorry.";
                                                            otherTalker.GetComponent<NewNpcScr>().Talk("itsok");
                                                            break;
                                                    }
                                                    break;
                                                case "electiondiogen":
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "";
                                                            break;
                                                        case 1:
                                                            tttext = "";
                                                            break;
                                                        case 2:
                                                            tttext = "";
                                                            break;
                                                    }
                                                    break;
                                                case "electionbronson":
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "";
                                                            break;
                                                        case 1:
                                                            tttext = "";
                                                            break;
                                                        case 2:
                                                            tttext = "";
                                                            break;
                                                    }
                                                    break;
                                                case "electiontherom":
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "";
                                                            break;
                                                        case 1:
                                                            tttext = "";
                                                            break;
                                                        case 2:
                                                            tttext = "";
                                                            break;
                                                    }
                                                    break;
                                            }
                                            break;
                                        case "coalitionSoldier":
                                            switch(controlScr.missionStatus["BanterCoalition"]){
                                                default:
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "Citizen.";
                                                            break;
                                                        case 1:
                                                            tttext = "Passing through?";
                                                            break;
                                                        case 2:
                                                            tttext = "Could use a break.";
                                                            break;
                                                    }
                                                    break;
                                            }
                                            break;
                                        case "refugeeCivilian":
                                            switch(controlScr.missionStatus["BanterRefugee"]){
                                                default:
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "I'm so tired. So hungry.";
                                                            break;
                                                        case 1:
                                                            tttext = "I have nowhere to go.";
                                                            break;
                                                        case 2:
                                                            tttext = "Don't know how much longer I can do this.";
                                                            break;
                                                    }
                                                    break;
                                            }
                                            break;
                                        case "dominionSoldier":
                                            switch(controlScr.missionStatus["BanterDominion"]){
                                                default:
                                                    ttchoice = UnityEngine.Random.Range(0, 2);
                                                    switch (ttchoice)
                                                    {
                                                        default:
                                                        case 0:
                                                            tttext = "Hope you can carry your weight.";
                                                            break;
                                                        case 1:
                                                            tttext = "Always on guard.";
                                                            break;
                                                        case 2:
                                                            tttext = "Another day, another day.";
                                                            break;
                                                    }
                                                    break;
                                                
                                            }
                                            break;
                                    }
                                }
                            }
                            
                            break;
                        case "banter":
                            switch (objName)
                            {
                                default:
                                    tttext = "What?";
                                    break;
                                case "coalitionCivilian":
                                    switch(controlScr.missionStatus["BanterCoalition"]){
                                        default:
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "I've had enough of this town, but nowhere else to go.";
                                                    break;
                                                case 1:
                                                    tttext = "Crazy world.";
                                                    break;
                                                case 2:
                                                    tttext = "You're making me nervous.";
                                                    break;
                                            }
                                            break;
                                        case "electiondiogen":
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "";
                                                    break;
                                                case 1:
                                                    tttext = "";
                                                    break;
                                                case 2:
                                                    tttext = "";
                                                    break;
                                            }
                                            break;
                                        case "electionbronson":
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "";
                                                    break;
                                                case 1:
                                                    tttext = "";
                                                    break;
                                                case 2:
                                                    tttext = "";
                                                    break;
                                            }
                                            break;
                                        case "electiontherom":
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "";
                                                    break;
                                                case 1:
                                                    tttext = "";
                                                    break;
                                                case 2:
                                                    tttext = "";
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "coalitionSoldier":
                                    switch(controlScr.missionStatus["BanterCoalition"]){
                                        default:
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "Citizen.";
                                                    break;
                                                case 1:
                                                    tttext = "Passing through?";
                                                    break;
                                                case 2:
                                                    tttext = "Could use a break.";
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "refugeeCivilian":
                                    switch(controlScr.missionStatus["BanterRefugee"]){
                                        default:
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "I'm so tired. So hungry.";
                                                    break;
                                                case 1:
                                                    tttext = "I have nowhere to go.";
                                                    break;
                                                case 2:
                                                    tttext = "Don't know how much longer I can do this.";
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "dominionSoldier":
                                    switch(controlScr.missionStatus["BanterDominion"]){
                                        default:
                                            ttchoice = UnityEngine.Random.Range(0, 2);
                                            switch (ttchoice)
                                            {
                                                default:
                                                case 0:
                                                    tttext = "Hope you can carry your weight.";
                                                    break;
                                                case 1:
                                                    tttext = "Always on guard.";
                                                    break;
                                                case 2:
                                                    tttext = "Another day, another day.";
                                                    break;
                                            }
                                            break;
                                        
                                    }
                                    break;
                            }
                            break;
                        case "point":
                            switch (objName)
                            {
                                default:
                                    break;
                                case "random":
                                    tttext = "What's up?";
                                    ttchoice = UnityEngine.Random.Range(0, 1);
                                    ttnew = tt + ttchoice.ToString();
                                    break;
                                case "coalitionCivilian":
                                    if (controlScr.repCoalition > 0)
                                    {
                                        ttchoice = UnityEngine.Random.Range(0, 2);
                                        ttnew = tt + ttchoice.ToString();
                                        switch (ttchoice)
                                        {
                                            default:
                                            case 0:
                                                tttext = "Why are you pointing that at me?";
                                                break;
                                            case 1:
                                                tttext = "Be careful with that.";
                                                break;
                                            case 2:
                                                tttext = "You're making me nervous.";
                                                break;
                                        }
                                    }
                                    break;
                                case "coalitionSoldier":
                                    if (controlScr.repCoalition > 0)
                                    {
                                        ttchoice = UnityEngine.Random.Range(0, 2);
                                        ttnew = tt + ttchoice.ToString();
                                        switch (ttchoice)
                                        {
                                            default:
                                            case 0:
                                                tttext = "Don't point that at me.";
                                                break;
                                            case 1:
                                                tttext = "Cut it out.";
                                                break;
                                            case 2:
                                                tttext = "Get away from me.";
                                                break;
                                        }
                                    }
                                    break;
                                case "refugeeCivilian":
                                    if (controlScr.repCoalition > 0)
                                    {
                                        ttchoice = UnityEngine.Random.Range(0, 2);
                                        ttnew = tt + ttchoice.ToString();
                                        switch (ttchoice)
                                        {
                                            default:
                                            case 0:
                                                tttext = "I have nothing to lose anyway.";
                                                break;
                                            case 1:
                                                tttext = "You'd be doing me a favor.";
                                                break;
                                            case 2:
                                                tttext = "Just do it already.";
                                                break;
                                        }
                                    }
                                    break;
                                case "alhareanSoldier":
                                    if (controlScr.repAlharean > 0)
                                    {
                                        ttchoice = UnityEngine.Random.Range(0, 2);
                                        ttnew = tt + ttchoice.ToString();
                                        switch (ttchoice)
                                        {
                                            default:
                                            case 0:
                                                tttext = "Human.";
                                                break;
                                            case 1:
                                                tttext = "Energy.";
                                                break;
                                            case 2:
                                                tttext = "Transformation.";
                                                break;
                                        }
                                    }
                                    break;
                                case "dominionSoldier":
                                    if (controlScr.repDominion > 0)
                                    {
                                        ttchoice = UnityEngine.Random.Range(0, 2);
                                        ttnew = tt + ttchoice.ToString();
                                        switch (ttchoice)
                                        {
                                            default:
                                            case 0:
                                                tttext = "Put that down.";
                                                break;
                                            case 1:
                                                tttext = "Give me a reason.";
                                                break;
                                            case 2:
                                                tttext = "Back up, civilian.";
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case "hello":
                            switch(objName){
                                default:
                                    break;
                                case "random":
                                    tttext="What's up?";
                                    ttchoice=UnityEngine.Random.Range(0,1);
                                    ttnew=tt+ttchoice.ToString();
                                    break;
                                case "coalitionCivilian":
                                    if(controlScr.repCoalition>0){
                                        ttchoice=UnityEngine.Random.Range(0,2);
                                        ttnew=tt+ttchoice.ToString();
                                        switch(ttchoice){
                                            default:
                                            case 0:
                                                if(controlScr.repCoalition>10){
                                                    tttext="Welcome friend.";
                                                }else{
                                                    tttext="What's up?";
                                                }
                                                
                                                break;
                                            case 1:
                                                if(controlScr.repCoalition>10){
                                                    tttext="Glad to have you here.";
                                                }else{
                                                    tttext="Hello there.";
                                                }
                                                
                                                break;
                                            case 2:
                                                if(controlScr.repCoalition>10){
                                                    tttext="Greetings.";
                                                }else{
                                                    tttext="Mhm.";
                                                }
                                                
                                                break;
                                        }
                                    }
                                    break;
                                case "coalitionSoldier":
                                    if(controlScr.repCoalition>0){
                                        ttchoice=UnityEngine.Random.Range(0,2);
                                        ttnew=tt+ttchoice.ToString();
                                        switch(ttchoice){
                                            default:
                                            case 0:
                                                tttext="Watch your step.";
                                                break;
                                            case 1:
                                                tttext="Careful.";
                                                break;
                                            case 2:
                                                tttext="Keep your distance.";
                                                break;
                                        }
                                    }
                                    break;
                                case "refugeeCivilian":
                                    if (controlScr.repCoalition > 0)
                                    {
                                        ttchoice = UnityEngine.Random.Range(0, 2);
                                        ttnew = tt + ttchoice.ToString();
                                        switch (ttchoice)
                                        {
                                            default:
                                            case 0:
                                                tttext = "Help us, please.";
                                                break;
                                            case 1:
                                                tttext = "I don't know how much more I can take.";
                                                break;
                                            case 2:
                                                tttext = "I should have stayed home.";
                                                break;
                                        }
                                    }
                                    break;
                                case "dominionSoldier":
                                    if(controlScr.repDominion>0){
                                        ttchoice=UnityEngine.Random.Range(0,2);
                                        ttnew=tt+ttchoice.ToString();
                                        switch(ttchoice){
                                            default:
                                            case 0:
                                                tttext="Mind your behavior.";
                                                break;
                                            case 1:
                                                tttext="Keep your hands where I can see them.";
                                                break;
                                            case 2:
                                                tttext="Keep moving, civilian.";
                                                break;
                                        }
                                    }
                                    break;
                                
                            }
                            break;
                    }
                }
                if(objSpecific){
                    ttnew="Sound\\"+objName+"\\"+ttnew;
                }else{
                    ttnew="Sound\\"+ttnew;
                }
                switch(ttnew){
                    default:
                        if((AudioClip)Resources.Load(ttnew)!=null){
                            soundsTalk.clip=(AudioClip)Resources.Load(ttnew);
                            controlScr.ShowMessage(tttext,"normal",2f);
                            soundsTalk.Play();
                        }else{
                            if(!string.IsNullOrEmpty(tttext)){
                                controlScr.ShowMessage(tttext,"normal",2f,true);
                            }
                        }
                        break;
                }
                talkTimer=UnityEngine.Random.Range(navMin,navMax);
            }
        }
     }

    private void Generate(string genType, float genSize)
    {
        Mesh mesh;
        float xSize = genSize;
        float ySize = genSize;
        float zSize = genSize;
        //int dnatype;
        int skinType = UnityEngine.Random.Range(0, 2);
        //front is 0,1,2,3 back is 4,5,6,7
        //
        Vector3
            ver0,
            ver1,
            ver2,
            ver3,
            ver4,
            ver5,
            ver6,
            ver7;
        mesh = new Mesh();
        ver0 =
            new Vector3(UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                0 - ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                0);
        ver1 =
            new Vector3(xSize + UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                0 - ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                0);
        ver2 =
            new Vector3(UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                0);
        ver3 =
            new Vector3(xSize + UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                0);
        ver4 =
            new Vector3(UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                0 - ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                zSize);
        ver5 =
            new Vector3(xSize + UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                0 - ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                zSize + UnityEngine.Random.Range(-zSize / 2f, zSize / 2f));
        ver6 =
            new Vector3(UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                zSize);
        ver7 =
            new Vector3(xSize + UnityEngine.Random.Range(-xSize / 2f, xSize / 2f),
                ySize + UnityEngine.Random.Range(-ySize / 2f, ySize / 2f),
                zSize);
        Vector3[] vertices =
            new Vector3[8] { ver0, ver1, ver2, ver3, ver4, ver5, ver6, ver7 };
        mesh.vertices = vertices;
        int[] tris =
            new int[36]
            {
                //front
                0,
                2,
                1,
                2,
                3,
                1,
                //back
                5,
                7,
                4,
                7,
                6,
                4,
                //left
                4,
                6,
                0,
                6,
                2,
                0,
                //right
                1,
                3,
                5,
                3,
                7,
                5,
                //top
                2,
                6,
                3,
                6,
                7,
                3,
                //bottom
                4,
                0,
                5,
                0,
                1,
                5
            };
        mesh.triangles = tris;
        Vector3[] normals =
            new Vector3[8]
            {
                Vector3.forward,
                Vector3.forward,
                Vector3.forward,
                Vector3.forward,
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back
            };
        mesh.normals = normals;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        if(head!=null){
            head.GetComponent<MeshFilter>().mesh = mesh;
        }
        switch(genType){
            default:
                /*
                Color tmpCol = Random.ColorHSV();
                chest.GetComponent<MeshRenderer>().material = controlScr.matDefault;
                legLeft.GetComponent<MeshRenderer>().material = controlScr.matDefault;
                legRight.GetComponent<MeshRenderer>().material = controlScr.matDefault;
                chest.GetComponent<MeshRenderer>().material.color = tmpCol;
                legLeft.GetComponent<MeshRenderer>().material.color = tmpCol;
                legRight.GetComponent<MeshRenderer>().material.color = tmpCol;
                */
                break;
        }
    }

    void SetCount(int vi){
        controlScr.countActive+=vi;
        if(countMe){
            switch(objTeam){
                default:
                break;
                case "dominion":
                    controlScr.countDominion+=vi;
                    break;
                case "zerran":
                    controlScr.countZerran+=vi;
                    break;
                case "unf":
                    controlScr.countUnf+=vi;
                    break;
                case "coalition":
                    controlScr.countCoalition+=vi;
                    break;
                case "collective":
                    controlScr.countCollective+=vi;
                    break;
                case "bandit":
                    controlScr.countBandits+=vi;
                    break;
                case "animal":
                    controlScr.countAnimals+=vi;
                    break;
                case "alharean":
                    controlScr.countAlharean+=vi;
                    break;
            }
        }
    }

    void SetActive(bool ia){
        isActive=ia;
        if (meshRens.Count > 0)
        {   
            foreach (MeshRenderer mren in meshRens)
            {
                mren.enabled = ia;
            }
        }
        if (skinnedMeshRens.Count > 0)
        {
            foreach (SkinnedMeshRenderer smren in skinnedMeshRens)
            {
                smren.enabled = ia;
            }
        }
        if(myAnimation!=null){
            myAnimation.enabled=ia;
        }
        if(myAnimator!=null){
            myAnimator.enabled=ia;
        }
    }

    public void ChangeNavTimer(float nt){
        animOverrideTimer=nt;
        navTimer+=nt;
    }

    void InteractWith(GameObject intObj){
        if((intObj!=null)&&(intObj.GetComponent<interactiveScr>()!=null)){
            intScr=intObj.GetComponent<interactiveScr>();
            intScr.iObj=this.gameObject;
            intScr.iObjScr=this.gameObject.GetComponent<NewNpcScr>();
            intScr.Interact();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        /*
        if (CompareTag("animal"))
        {
            if (other.gameObject.CompareTag("animal"))
            {
                if (other.gameObject.GetComponent<NewNpcScr>().objName == objName)
                {
                    ranFloat = UnityEngine.Random.Range(0f, 1f);
                    if (ranFloat < 0.05f)
                    {
                        if (controlScr.countActive < 30)
                        {
                            controlScr.SpawnObj(objName, transform.position - transform.forward, transform.rotation);
                        }
                    }
                }
            }
        }*/
    }

    void OnTriggerEnter(Collider other){
        /*
        if(other.gameObject.GetComponent<NewNpcScr>()!=null){
            if (other.gameObject.GetComponent<NewNpcScr>().objName == objName)
            {
                if(playerObj!=null){
                    if(Vector3.Distance(transform.position,playerObj.transform.position)<navRange){
                        otherTalker=other.gameObject;
                        otherTalkerStarter=this.gameObject;
                        otherTalkerReceiver=otherTalker;
                        otherTalker.GetComponent<NewNpcScr>().otherTalkerStarter=this.gameObject;
                        otherTalker.GetComponent<NewNpcScr>().otherTalkerReceiver=otherTalker;
                        Talk("banterally");
                    }
                }
            }
        }*/
        if(other.CompareTag("activezone")){
            if(!isActive){
                SetActive(true);
                SetCount(1);
            }
        }
        if (other.CompareTag("cube"))
        {
            if(myTent==null){
                if(other.gameObject.name=="TentCoalition"){
                    myTent=other.gameObject;
                }
            }
        }
        if (other.CompareTag("inventorytag"))
        {
            switch(objName){
                default:
                    break;
                case "TheronMarsden":
                    if(other.GetComponent<inventoryItemScr>()!=null){
                        if(other.GetComponent<inventoryItemScr>().iname=="AlhareanArtifact"){
                            switch(controlScr.missionStatus["TheArtifact"]){
                                default:
                                    break;
                                case "endhelpyes":
                                    controlScr.UpdateMission("TheronMarsden","startartifacttheron","startartifacttheron");
                                    controlScr.UpdateMission("TheArtifact","artifacttheronhelp","",
                                    "Talk to Theron about the Alharean Artifact he recovered.","AlhareanArtifact","The Artifact","show");
                                    break;
                                case "endhelpno":
                                    controlScr.UpdateMission("TheronMarsden","startartifacttheron","startartifacttheron");
                                    controlScr.UpdateMission("TheArtifact","artifacttheronalone","",
                                    "Theron about the Alharean Artifact he recovered.","AlhareanArtifact","The Artifact","show");
                                    break;
                            }
                        }
                    }
                    break;
                case "coalitionCivilian":
                    if(other.GetComponent<TriggerScr>()!=null){
                        if(other.GetComponent<TriggerScr>().trigType=="poster"){
                            switch(other.GetComponent<TriggerScr>().trigName){
                                default:
                                    break;
                                case "PosterBronson":
                                    controlScr.delarusElectionKeeling+=1;
                                    break;
                                case "PosterDiogen":
                                    controlScr.delarusElectionDiogen+=1;
                                    break;
                                case "PosterTherom":
                                    controlScr.delarusElectionScandos+=1;
                                    break;
                            }
                        }
                    }
                    break;
            }
            
        }
        if (other.CompareTag("waterzone"))
        {
            if (!inWater)
            {
                inWater = true;
            }
        }
        //Routine, needs
        if (other.CompareTag("food"))
        {
            ChangeRoutine();
            hunger=100;
            //Destroy(other.gameObject);
        }
        if (other.CompareTag("sleep"))
        {
            canMove=false;
        }
        //End routines
        if (other.CompareTag("alerttag")){
            if (!inBattle)
            {
                aimTargetOverride=new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                if(!isMoving){
                    aimTargetOverrideTimer = 3f;
                }else{
                    aimTargetOverrideTimer = 1f;
                }
                switch(other.gameObject.GetComponent<destroyScr>().owner){
                    default:
                        break;
                    case "Player":
                        switch(objTeam){
                            default:
                                break;
                            case "dominion":
                                switch(playerScr.curArea){
                                    default:
                                        break;
                                    case "DominionPrime":
                                    case "Jormad":
                                        Talk("stopit");
                                        break;
                                }
                                break;
                            case "alharean":
                                 switch(playerScr.curArea){
                                    default:
                                        Talk("stopit");
                                        break;
                                }
                                break;
                            case "coalition":
                                switch(playerScr.curArea){
                                    default:
                                        break;
                                    case "Delarus":
                                    case "NewCentaurus":
                                        Talk("stopit");
                                        break;
                                }
                                break;
                        }
                        
                        break;
                }
            }
            switch(other.GetComponent<destroyScr>().name){
                default:
                    break;
                case "grenade":
                    if(!inBattle){
                        inBattle = true;
                    }
                    impulseTimer+=5f;
                    if(impulseTimer>5f){
                        DodgeRelativePosition(other.gameObject.transform.position);
                    }
                    break;
            }
        }

        if(other.CompareTag("flagtag")){
            if(controlScr.flagHolder==null){
                other.gameObject.GetComponent<flagScr>().SetHolder(this.gameObject,objTeam);
                holdingFlag=true;
                pathTimer=0f;
                navOverrideTimer=0f;
                Navigate("flag");
            }
        }
        if (other.CompareTag("tornado")){
            moveDir.y = 25f;
        }
        if(other.CompareTag("Player")){
            nearPlayer=true;
            if ((!inBattle)&&(!playerScr.isTalking))
            {
                if (reputation > 0)
                {
                    if (playerScr.wepDrawn)
                    {
                        Talk("point", true);
                    }
                    else
                    {
                        Talk("hello", true);
                    }
                    if(!isMoving){
                        aimTargetOverride=playerObj.transform.position;
                        aimTargetOverrideTimer+=1f;
                    }
                }
                switch (navAction)
                {
                    default:
                        break;
                    case "hold":
                        aimTargetOverride=other.gameObject.transform.position;
                        aimTargetOverrideTimer += 1;
                        break;
                }
            }
        }
        /*
        if(other.CompareTag("hurtzone")){
            rechargeTimer=10f;
            shields-=100f;
            if(shields<=0){
                hp-=100f;
            }
            deathType = "explosion";
        }*/
        
        if(other.CompareTag("acidrain")){
            rechargeTimer=10f;
            if(shields>0f){
                shields-=100f;
            }
            if(shields<=0f){
                hp-=50f;
            }
            deathType = "explosion";
        }
        if(other.CompareTag("weapon")){
            if((canSwapWep)&&(canSwap)){
                tempSwap=other.gameObject;
                if(tempSwap.GetComponent<pickWepScr>()!=null){
                    wepScr=tempSwap.GetComponent<pickWepScr>();
                    swapInt=wepScr.wepValue;
                    switch(swapInt){
                        default:
                            SwapWep(swapInt);
                            Destroy(tempSwap);
                            break;
                        case 99://Grenade
                            greCount+=1;
                            Destroy(tempSwap);
                            break;
                        case 98://Plasma grenade
                            greCount+=1;
                            Destroy(tempSwap);
                            break;
                    }
                }
            }
        }
        if(other.CompareTag("interactive")){
            if(interactTimer<=0f){
                InteractWith(other.gameObject);
            }
        }
    }
    void OnTriggerExit(Collider other){
        if(other.CompareTag("activezone")){
            if(isActive){
                SetActive(false);
                SetCount(-1);
            }
        }
        if (other.CompareTag("waterzone"))
        {
            if (inWater)
            {
                inWater = false;
                if (!isUnderwater)
                {
                    waterAir = 10f;
                }
            }
        }
        if (other.CompareTag("Player")){
            nearPlayer=false;
        }
        if (other.CompareTag("weapon"))
        {
            swapInt = 100;//Neutral, no weapon
        }
    }
}
