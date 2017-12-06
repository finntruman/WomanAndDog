using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDog : MonoBehaviour {

    SelectorNode m_AI;

	// Use this for initialization
	void Start ()
    {
        // -- ROOT AND ITS CHILDREN --
        // Root node selector
        m_AI = new SelectorNode();

        // 1st node of Root
        Condition whistleCondition = new Condition(Conditions.hasWhistled);
        ConditionNode whistle = new ConditionNode(whistleCondition, this);

        // 2nd node of Root
        Condition inSphereCondition = new Condition(Conditions.inSphere);
        ConditionNode inSphere = new ConditionNode(inSphereCondition, this);

        // 3rd node of Root
        StochasticNode runFaster = new StochasticNode();

        // Adding children to Root
        m_AI.AddNode(whistle);
        m_AI.AddNode(inSphere);
        m_AI.AddNode(runFaster);

        // -- CHILDREN OF WHISTLE CONDITION (1ST CHILD OF ROOT) --
        Action approachAction = new Action(Actions.ApproachPlayer);
        ActionNode approach = new ActionNode(approachAction, this);
        whistle.AddNode(approach);

        // -- CHILDREN OF IN-SPHERE CONDITION (2ND CHILD OF ROOT) --
        Condition interactingCondition = new Condition(Conditions.interacting);
        ConditionNode interacting = new ConditionNode(interactingCondition, this);
        inSphere.AddNode(interacting);

        // -- CHILDREN OF RUN-FASTER CONDITION (3RD CHILD OF ROOT) --
        SequenceNode runAhead = new SequenceNode();
        SequenceNode runWith = new SequenceNode();
        runFaster.AddNode(runAhead);
        runFaster.AddNode(runWith);

        // -- CHILDREN OF INTERACTING CONDITION NODE (CHILD OF IN-SPHERE CONDITION) --
        SequenceNode sitThenHunt = new SequenceNode();
        SequenceNode idle = new SequenceNode();
        interacting.AddNode(sitThenHunt);
        interacting.AddNode(idle);

        // -- CHILDREN OF SIT-THEN-HUNT SEQUENCE (CHILD OF SIT-THEN-HUNT SEQUENCE) --
        Action sitAction = new Action(Actions.Sit);
        ActionNode sit = new ActionNode(sitAction, this);
        SequenceNode huntSequence = new SequenceNode();
        sitThenHunt.AddNode(sit);
        sitThenHunt.AddNode(huntSequence);

        // -- CHILDREN OF IDLE SEQUENCE (CHILD OF IDLE SEQUENCE) --
        Action sniffAction = new Action(Actions.Sniff);
        ActionNode randomSpot = new ActionNode(sniffAction, this);
        Condition tenMinutesCondition = new Condition(Conditions.tenMinutes);
        ConditionNode tenMinutes = new ConditionNode(tenMinutesCondition, this);
        idle.AddNode(randomSpot);
        idle.AddNode(tenMinutes); // joint child

        // -- CHILDREN OF RUN-AHEAD SEQUENCE (CHILD OF RUN-FASTER CONDITION) --
        Action runFastAction = new Action(Actions.RunFast);
        ActionNode runFast = new ActionNode(runFastAction, this);
        Action waitAction = new Action(Actions.Wait);
        ActionNode turnAndWait = new ActionNode(waitAction, this);
        runAhead.AddNode(runFast);
        runAhead.AddNode(tenMinutes); // joint child
        tenMinutes.AddNode(huntSequence);
        runAhead.AddNode(turnAndWait);

        // -- CHILDREN OF RUN-WITH SEQUENCE (CHILD OF RUN-FASTER CONDITION) --
        Action runWithAction = new Action(Actions.RunWith);
        ActionNode runNear = new ActionNode(runWithAction, this);
        runWith.AddNode(runNear);

        // -- CHILDREN OF HUNT SEQUENCE (CHILD OF SIT-THEN-HUNT SEQUENCE AND TEN-MINUTES CONDITION) --
        Action dissapearAction = new Action(Actions.Dissapear);
        ActionNode dissapear = new ActionNode(dissapearAction, this);
        Action huntedAction = new Action(Actions.Hunted);
        ActionNode hunted = new ActionNode(huntedAction, this);
        huntSequence.AddNode(dissapear);
        huntSequence.AddNode(hunted);

        // START
        m_AI.StartCoroutine(m_AI.Execute());
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
