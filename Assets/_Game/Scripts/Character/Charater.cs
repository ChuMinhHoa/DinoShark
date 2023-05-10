using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Charater : MonoBehaviour
{
    public StateMachine<Charater> StateMachine { get { return m_StateMachine; } }
    protected StateMachine<Charater> m_StateMachine;
    public CharaterGlobalState charaterGlobalState = new CharaterGlobalState();
    public CharaterIdleState charaterIdleState = new CharaterIdleState();
    public CharaterMoveState charaterMoveState = new CharaterMoveState();
    public CharaterDoWork charaterDoWorkState = new CharaterDoWork();
    public CharaterOurRestaurant charaterOurRestaurantState = new CharaterOurRestaurant();

    public Animator m_Animator;
    [HideInInspector] public string ONIDLE = "OnIdle";
    [HideInInspector] public string ONMOVE = "OnMove";
    [HideInInspector] public string ONWORK = "OnWork";
    [HideInInspector] public string ONRUN = "OnRun";
    [HideInInspector] public string ONEAT = "OnEat";
    public List<SkinnedMeshRenderer> meshs;
    public SkinnedMeshRenderer face;
    public NavMeshAgent agent;
    public Transform toMovePosition;
    public TimeFill timeIndicator;

    public GameObject myEgg;
    public GameObject smokeEffect;
    public GameObject myBody;
    public Transform eggPoint;
    public bool eggMode;
    void Awake()
    {
        InitStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPauseGame) return;
        m_StateMachine.Update();
    }

    public void OnEnableMove()
    {
        if (m_Animator) m_Animator.SetBool(ONMOVE, true);
        if (m_Animator) m_Animator.SetBool(ONIDLE, false);
        if (m_Animator) m_Animator.SetBool(ONWORK, false);
        if (m_Animator) m_Animator.SetBool(ONEAT, false);
    }
    public void OnDisableMove()
    {
        if (m_Animator) m_Animator.SetBool(ONMOVE, false);
        if (m_Animator) m_Animator.SetBool(ONRUN, false);
    }

    public bool IsFinishMoveOnNavemesh()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected virtual void InitStateMachine()
    {
        m_StateMachine = new StateMachine<Charater>(this);
        m_StateMachine.SetCurrentState(charaterIdleState);
        m_StateMachine.SetGlobalState(CharaterGlobalState.Instance);
    }

    public virtual void OnIdleStart()
    {
        if (m_Animator) m_Animator.SetBool(ONIDLE, true);
    }
    public virtual void OnIdleExecute()
    {
    }
    public virtual void OnIdleExit()
    {
        if (m_Animator) m_Animator.SetBool(ONIDLE, false);
    }

    public virtual void OnMoveStart()
    {
        agent.enabled = true;
        OnEnableMove();
        if (ProfileManager.Instance.playerData.boostSave.GetAdsBoostRemain() > 0)
        {
            if (m_Animator) m_Animator.SetBool(ONRUN, true);
        }
        agent.SetDestination(toMovePosition.position);
    }
    public virtual void OnMoveExecute()
    {
        
    }
    public virtual void OnMoveExit()
    {
        agent.enabled = false;
        OnDisableMove();
    }

    public virtual void OnDoWorkStart()
    {
        if (m_Animator) m_Animator.SetBool(ONWORK, true);
    }
    public virtual void OnDoWorkExecute() { }
    public virtual void OnDoWorkExit() 
    {
        if (m_Animator) m_Animator.SetBool(ONWORK, false);
    }

    public virtual void OutRestaurantStart()
    {
        agent.enabled = true;
        OnEnableMove();
    }
    public virtual void OutRestaurantExecute() { }

    public virtual void OutRestaurantExit() 
    {
        agent.enabled = false;
        OnDisableMove();
    }

    public virtual void EggMode() {
        
    }

    public virtual void EggBroke() { }

    public virtual IEnumerator IE_WaitAnim()
    {
        return null;
    }
}
