public class CharaterGlobalState : State<Charater>
{
    private static CharaterGlobalState m_Instance = null;
    public static CharaterGlobalState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CharaterGlobalState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Charater go)
    {
    }
    public override void Execute(Charater go)
    {
    }
    public override void Exit(Charater go)
    {
    }

}



public class CharaterIdleState : State<Charater>
{
    private static CharaterIdleState m_Instance = null;
    public static CharaterIdleState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CharaterIdleState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Charater go)
    {
        go.OnIdleStart();
    }
    public override void Execute(Charater go)
    {
        go.OnIdleExecute();
    }
    public override void Exit(Charater go)
    {
        go.OnIdleExit();
    }
}

public class CharaterMoveState : State<Charater>
{
    private static CharaterMoveState m_Instance;
    public static CharaterMoveState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CharaterMoveState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Charater go)
    {
        go.OnMoveStart();
    }
    public override void Execute(Charater go)
    {
        go.OnMoveExecute();
    }
    public override void Exit(Charater go)
    {
        go.OnMoveExit();
    }
}

public class CharaterDoWork : State<Charater>
{
    private static CharaterDoWork m_Instance;
    public static CharaterDoWork Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CharaterDoWork();
            }
            return m_Instance;
        }
    }
    public override void Enter(Charater go)
    {
        go.OnDoWorkStart();
    }
    public override void Execute(Charater go)
    {
        go.OnDoWorkExecute();
    }
    public override void Exit(Charater go)
    {
        go.OnDoWorkExit();
    }
}

public class CharaterOurRestaurant : State<Charater>
{
    private static CharaterOurRestaurant m_Instance;
    public static CharaterOurRestaurant Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CharaterOurRestaurant();
            }
            return m_Instance;
        }
    }
    public override void Enter(Charater go)
    {
        go.OutRestaurantStart();
    }
    public override void Execute(Charater go)
    {
        go.OutRestaurantExecute();
    }
    public override void Exit(Charater go)
    {
        go.OutRestaurantExit();
    }
}

public enum CharIdleAct
{
    OnHello,
    OnJump,
    OnYes,
}