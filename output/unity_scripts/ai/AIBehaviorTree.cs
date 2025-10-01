using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIBehaviorTree : MonoBehaviour
{
    [Header("Behavior Tree")]
    public BTNode rootNode;
    public float updateFrequency = 0.1f;
    
    [Header("Current State")]
    public string currentAction;
    public BTNode.Status treeStatus;
    
    private AICharacter aiCharacter;
    private float lastUpdateTime;
    
    void Start()
    {
        aiCharacter = GetComponent<AICharacter>();
        InitializeBehaviorTree();
        lastUpdateTime = Time.time;
    }
    
    void Update()
    {
        if (Time.time - lastUpdateTime >= updateFrequency)
        {
            UpdateBehaviorTree();
            lastUpdateTime = Time.time;
        }
    }
    
    void InitializeBehaviorTree()
    {
        // Create behavior tree structure
        rootNode = new SequenceNode("Root");
        
        // Health check priority
        var healthCheck = new SelectorNode("Health Check");
        healthCheck.AddChild(new CheckLowHealthNode(aiCharacter));
        healthCheck.AddChild(new CheckAmmoNode(aiCharacter));
        
        // Combat behavior
        var combatBehavior = new SelectorNode("Combat Behavior");
        combatBehavior.AddChild(new CheckEnemyInSightNode(aiCharacter));
        combatBehavior.AddChild(new CheckLastKnownPositionNode(aiCharacter));
        combatBehavior.AddChild(new PatrolNode(aiCharacter));
        
        // Mission objectives
        var missionBehavior = new SelectorNode("Mission Behavior");
        missionBehavior.AddChild(new CompleteObjectiveNode(aiCharacter));
        missionBehavior.AddChild(new FollowTeamNode(aiCharacter));
        
        rootNode.AddChild(healthCheck);
        rootNode.AddChild(combatBehavior);
        rootNode.AddChild(missionBehavior);
        
        Debug.Log("AI Behavior Tree initialized for " + aiCharacter.characterName);
    }
    
    void UpdateBehaviorTree()
    {
        if (rootNode != null)
        {
            treeStatus = rootNode.Process();
            currentAction = rootNode.GetCurrentAction();
        }
    }
    
    // Base Node Class
    public abstract class BTNode
    {
        public string name;
        protected List<BTNode> children = new List<BTNode>();
        protected string currentAction;
        
        public enum Status { Running, Success, Failure }
        
        public BTNode(string nodeName)
        {
            name = nodeName;
        }
        
        public void AddChild(BTNode child)
        {
            children.Add(child);
        }
        
        public string GetCurrentAction()
        {
            return currentAction;
        }
        
        public abstract Status Process();
    }
    
    // Sequence Node (AND logic)
    public class SequenceNode : BTNode
    {
        public SequenceNode(string name) : base(name) { }
        
        public override Status Process()
        {
            foreach (var child in children)
            {
                Status childStatus = child.Process();
                if (childStatus != Status.Success)
                {
                    currentAction = child.GetCurrentAction();
                    return childStatus;
                }
            }
            currentAction = "Sequence Complete";
            return Status.Success;
        }
    }
    
    // Selector Node (OR logic)
    public class SelectorNode : BTNode
    {
        public SelectorNode(string name) : base(name) { }
        
        public override Status Process()
        {
            foreach (var child in children)
            {
                Status childStatus = child.Process();
                if (childStatus != Status.Failure)
                {
                    currentAction = child.GetCurrentAction();
                    return childStatus;
                }
            }
            currentAction = "All Options Failed";
            return Status.Failure;
        }
    }
    
    // Condition Nodes
    public class CheckLowHealthNode : BTNode
    {
        private AICharacter character;
        
        public CheckLowHealthNode(AICharacter aiChar) : base("Check Low Health")
        {
            character = aiChar;
        }
        
        public override Status Process()
        {
            if (character.health < character.maxHealth * 0.3f)
            {
                currentAction = "Seeking Cover - Low Health";
                return Status.Success;
            }
            currentAction = "Health OK";
            return Status.Failure;
        }
    }
    
    public class CheckEnemyInSightNode : BTNode
    {
        private AICharacter character;
        
        public CheckEnemyInSightNode(AICharacter aiChar) : base("Check Enemy In Sight")
        {
            character = aiChar;
        }
        
        public override Status Process()
        {
            // Implementation for checking if enemy is in sight
            if (character.HasEnemyInSight())
            {
                currentAction = "Engaging Enemy";
                return Status.Success;
            }
            currentAction = "No Enemy in Sight";
            return Status.Failure;
        }
    }
    
    // Action Nodes
    public class PatrolNode : BTNode
    {
        private AICharacter character;
        
        public PatrolNode(AICharacter aiChar) : base("Patrol")
        {
            character = aiChar;
        }
        
        public override Status Process()
        {
            character.Patrol();
            currentAction = "Patrolling Area";
            return Status.Running;
        }
    }
    
    public class FollowTeamNode : BTNode
    {
        private AICharacter character;
        
        public FollowTeamNode(AICharacter aiChar) : base("Follow Team")
        {
            character = aiChar;
        }
        
        public override Status Process()
        {
            character.FollowTeam();
            currentAction = "Following Team";
            return Status.Running;
        }
    }
    
    public class CompleteObjectiveNode : BTNode
    {
        private AICharacter character;
        
        public CompleteObjectiveNode(AICharacter aiChar) : base("Complete Objective")
        {
            character = aiChar;
        }
        
        public override Status Process()
        {
            character.CompleteObjective();
            currentAction = "Working on Objective";
            return Status.Running;
        }
    }
}
