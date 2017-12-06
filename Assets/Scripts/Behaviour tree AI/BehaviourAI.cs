using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    FALSE, RUNNING, TRUE, INACTIVE
}

class Conditions
{
    // This is where the condition functions go that are invoked
    public static bool hasWhistled(ControllerDog agent)
    {
        Debug.Log("Checking for whistle...");
        //return ControllerPlayer.g_player.m_whistled;
        return false;
    }

    public static bool inSphere(ControllerDog agent)
    {
        Debug.Log("Checking for sphere...");
        return false;
    }

    public static bool interacting(ControllerDog agent)
    {
        Debug.Log("Checking for interacting...");
        return false;
    }

    public static bool tenMinutes(ControllerDog agent)
    {
        Debug.Log("Checking minutes...");
        return false;
    }
}

class Actions
{
    public static State ApproachPlayer(ControllerDog agent)
    {
        Debug.Log("Approaching...");
        return State.FALSE;
    }

    public static State Sit(ControllerDog agent)
    {
        Debug.Log("Sitting...");
        return State.FALSE;
    }

    public static State Sniff(ControllerDog agent)
    {
        Debug.Log("Sniffing...");
        return State.FALSE;
    }

    public static State RunFast(ControllerDog agent)
    {
        Debug.Log("Running fast...");
        return State.FALSE;
    }

    public static State Wait(ControllerDog agent)
    {
        Debug.Log("Waiting...");
        return State.FALSE;
    }

    public static State RunWith(ControllerDog agent)
    {
        Debug.Log("Running with...");
        return State.FALSE;
    }

    public static State Dissapear(ControllerDog agent)
    {
        Debug.Log("Dissapearing...");
        return State.FALSE;
    }

    public static State Hunted(ControllerDog agent)
    {
        Debug.Log("Hunting...");
        return State.FALSE;
    }
}

public class Node : MonoBehaviour
{
    static public ControllerPlayer m_player;
    public ControllerDog m_agent;
    public List<Node> m_children;
    protected State m_state;

    public State GetState() { return m_state; }

    //public virtual State Execute()
    //{
    //    return State.FALSE;
    //}

    public virtual IEnumerator Execute()
    {
        m_state = State.FALSE;
        yield return null;
    }

    // Adds node to end (right) of list
    public void AddNode(Node node)
    {
        m_children.Add(node);
    }
}

public delegate bool Condition(ControllerDog agent);
public class ConditionNode : Node
{
    Condition m_condition;

    public ConditionNode(Condition condition, ControllerDog agent)
    {
        m_condition = condition;
        m_agent = agent;
    }

    public override IEnumerator Execute()
    //public override State Execute()
    {
        bool success = m_condition.Invoke(m_agent);
        if (success)
        {
            m_state = State.TRUE;
            //return State.TRUE;
            yield return null;
        }
        else
        {
            m_state = State.FALSE;
            //return State.FALSE;
            yield return null;
        }
    }
}

public class CompositeNode : Node
{
  
}

// Processes its child nodes in left to right order
// Suceeds only if ALL child nodes suceed
// If ANY child node fails, entire sequence fails
public class SequenceNode : CompositeNode
{
    public override IEnumerator Execute()
    //public override State Execute()
    {
        m_state = State.RUNNING;
        for (int i = 0; i < m_children.Count; i++)
        {
            if (m_children[i].GetState() != State.RUNNING)
            {
                yield return m_children[i].Execute();
                if (m_children[i].GetState() == State.FALSE)
                //if (m_children[i].Execute() == State.FALSE)
                {
                    m_state = State.FALSE;
                    //return m_state;
                    yield return null;
                }
            }
        }
        m_state = State.TRUE;
        yield return null;
    }
}

// Tries all of its child nodes in left to right order
// Succeeds if ANY child node suceeds
// Fails if ALL child nodes fail
public class SelectorNode : CompositeNode
{
    public override IEnumerator Execute()
    //public override State Execute()
    {
        m_state = State.RUNNING;
        for (int i = 0; i < m_children.Count; i++)
        {
            yield return m_children[i].Execute();
            if (m_children[i].GetState() == State.TRUE)
            {
                m_state = State.TRUE;
                yield return null;
            }
            //if (m_children[i].GetState() != State.RUNNING)
            //{
            //    if (m_children[i].Execute() == State.TRUE)
            //    {
            //        m_state = State.TRUE;
            //        return m_state;
            //    }
            //}
        }
        m_state = State.FALSE;
        //return m_state;
        yield return null;
    }
}

// Tries a random child node and succeeds if the random node does
public class StochasticNode : CompositeNode
{
    public override IEnumerator Execute()
    //public override State Execute()
    {
        int random = Random.Range(0, m_children.Count - 1);
        yield return m_children[random].Execute();
        m_state = (m_children[random].GetState());
        yield return null;
    }
}

public delegate State Action(ControllerDog agent);
public class ActionNode : Node
{
    Action m_action;

    public ActionNode(Action action, ControllerDog agent)
    {
        m_action = action;
        m_agent = agent;
    }

    public override IEnumerator Execute()
    //public override State Execute()
    {
        m_state = State.RUNNING;
        yield return m_action.Invoke(m_agent);
        //if (m_state != State.RUNNING)
        //{
        //    m_state = State.RUNNING;
        //    m_action.Invoke(m_agent);
        //}
        //return m_state;
    }
}