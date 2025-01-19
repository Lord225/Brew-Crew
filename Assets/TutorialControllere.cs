using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static OrderController;

public class TutorialControllere : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private OrderController orderController;
    private NavMeshPlayerController playerController;
    private Inventory playerInventory;

    private TextMeshProUGUI tutorialText;


    public TutorialArrow arrow;

    void Start()
    {
        orderController = GetComponent<OrderController>();

        playerController = GameObject.Find("Player").GetComponent<NavMeshPlayerController>();
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        tutorialText = GameObject.Find("TutText").GetComponent<TextMeshProUGUI>();
        
        arrow.hide();

        Debug.Assert(orderController != null);
        Debug.Assert(playerController != null);

        orderController.OnOrderListChange += OrderListChange;
        playerInventory.OnInventoryItemChanged += PlayerItemChange;
    }

    public class TutorialStep
    {
        public string name;
        public string description;
        public string nextStepTrigger;
    }

    List<TutorialStep> tutorialSteps = new List<TutorialStep> {
        new TutorialStep { 
            name = "Hide", 
            description = "Welcome aboard, rookie! First rule of the Crew: hide when clients show up. Just kidding... maybe. Get ready, a client is about to walk in!", 
            nextStepTrigger = "Client" 
        },
        new TutorialStep { 
            name = "Coffe", 
            description = "New client alert! They want an espresso. But first, you'll need to load some coffee beans into the machine. Consider this your initiation ritual.", 
            nextStepTrigger = "CoffePickup" 
        },
        new TutorialStep { 
            name = "CoffeMachine", 
            description = "Alright, rookie, time to put those beans into the coffee machine. Don't worry, it's not rocket science... or is it?", 
            nextStepTrigger = "AnyMachineReady" 
        },
        new TutorialStep { 
            name = "Cup", 
            description = "The machine's ready! Place the mug on it and start brewing by hitting [E]. It's like magic, but with caffeine.", 
            nextStepTrigger = "CupPickup" 
        },
        new TutorialStep { 
            name = "CoffeMachine", 
            description = "Nice work, kitten! Now watch the machine work its magic. Be patient; great coffee takes time. Just don't fall asleep while waiting, okay?", 
            nextStepTrigger = "AnyMachineBrewd" 
        },
        new TutorialStep { 
            name = "Table", 
            description = "Espresso! Deliver it to the client. Bonus points if you don't spill it everywhere. (Seriously, don't.)", 
            nextStepTrigger = "OrderDone" 
        },
        new TutorialStep { 
            name = "Hide", 
            description = "Nice work rookie! But not for too long... more caffeine-craving clients are on their way! Check Recipe Book for more recipes under [TAB]. If you need to craft someting. Just put items on same countertop.", 
            nextStepTrigger = "" 
        }
    };

    public int currentStepIndex = 0;

    void SetTutorialState(string name) {
        switch(name) {
            case "Hide":
                arrow.hide();
                break;
            case "Cup":
                arrow.Cup();
                break;
            case "Coffe":
                arrow.Coffe();
                break;
            case "Top":
                arrow.Top();
                break;
            case "CoffeMachine":
                arrow.CoffeMachine();
                break;
            case "Steamer":
                arrow.Steamer();
                break;
            case "Milk":
                arrow.Milk();
                break;
            case "Table":
                arrow.Table();
                break;
        }
    }

    void OrderListChange(List<Order> orders) {
        if(currentStepIndex >= tutorialSteps.Count) {
            return;
        }

        var currentStep = tutorialSteps[currentStepIndex];
        if(currentStep.nextStepTrigger == "OrderDone") {
            currentStepIndex++;
        }
    }

    void PlayerItemChange(GameObject newItem) {
        if(currentStepIndex >= tutorialSteps.Count) {
            return;
        }

        var currentStep = tutorialSteps[currentStepIndex];
        if(currentStep.nextStepTrigger == "CupPickup") {
            if(newItem != null) {
                if(newItem.TryGetComponent(out MugState state)) {
                    currentStepIndex++;
                }
            }
        } else if(currentStep.nextStepTrigger == "CoffePickup") {
            if(newItem != null) {
                if(newItem.TryGetComponent(out CoffeSeeds state)) {
                    currentStepIndex++;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentStepIndex >= tutorialSteps.Count) {
            SetTutorialState("Hide");
            return;
        }
        

        var currentStep = tutorialSteps[currentStepIndex];
        SetTutorialState(currentStep.name);
        tutorialText.text = currentStep.description;

        if(currentStep.nextStepTrigger == "Client") {
            var client = FindFirstObjectByType<ClientScript>();
            if(client != null) {
                currentStepIndex++;
            }
        } else if(currentStep.nextStepTrigger == "AnyMachineReady") {
            var machines = FindObjectsByType<CoffeMachine>(FindObjectsSortMode.None);
            var machineList = new List<CoffeMachine>(machines);
            
            // if any machine has status ready
            if(machineList.Exists(machine => machine.state == CoffeMachine.CoffeMachineState.CoffeInside)) { 
                currentStepIndex++;
            }
        } else if(currentStep.nextStepTrigger == "AnyMachineBrewd") {
            var machines = FindObjectsByType<CoffeMachine>(FindObjectsSortMode.None);
            var machineList = new List<CoffeMachine>(machines);
            
            // if any machine has status ready
            if(machineList.Exists(machine => machine.state == CoffeMachine.CoffeMachineState.Brewed)) { 
                currentStepIndex++;
            }
        } else if(currentStep.nextStepTrigger == "ClientWantsAmericano") {
            var client = FindFirstObjectByType<ClientScript>();
            if(client != null && client.order.requestedMug == MugState.State.Americano) {
                currentStepIndex++;
            }
        }

         
    }
}
