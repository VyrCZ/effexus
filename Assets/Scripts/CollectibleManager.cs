using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    public bool hasTp;
    public bool hasActivatorUp;
    public bool hasActivatorDown;

    public float interactDistance = 3f;

    Vector3 eyesOffset = new(0, 0.5f, 0);
    CameraController cameraController;
    UIManager ui;

    public LayerMask tpMask;
    public LayerMask wallsPlusTpMask;
    public LayerMask doorMask;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Collectible")){
            switch(other.GetComponent<Collectible>().type){
                case Collectible.Type.activatorUp:
                    hasActivatorUp = true;
                    ui.hasItem[0] = true;
                    break;
                case Collectible.Type.activatorDown:
                    hasActivatorDown = true;
                    ui.hasItem[1] = true;
                    break;
                case Collectible.Type.tp:
                    hasTp = true;
                    ui.hasItem[2] = true;
                    break;
            }
        }
    }

    private void Start() {
        cameraController = Camera.main.GetComponent<CameraController>();
        ui = FindObjectOfType<UIManager>();
    }

    private void Update() {
        ui.highlightedCrosshair = false;
        ui.pointingCrosshair = false;
        if(Physics.Raycast(transform.position + eyesOffset, cameraController.lookDirection, out RaycastHit hit, interactDistance)){
            if(hit.collider.CompareTag("Elevator")){
                var elevator = hit.collider.GetComponent<Elevator>();
                if(!elevator.active){
                    if((elevator.goingUp && hasActivatorUp) || (!elevator.goingUp && hasActivatorDown)){
                        ui.highlightedCrosshair = true;
                        if(Input.GetKeyDown(KeyCode.E)){
                            elevator.Activate();
                            if(elevator.goingUp){
                                hasActivatorUp = false;
                                ui.hasItem[0] = false;
                            }
                            else{
                                hasActivatorDown = false;
                                ui.hasItem[1] = false;
                            }
                        }
                    }
                }
            }
        }
        if(hasTp && Physics.Raycast(transform.position + eyesOffset, cameraController.lookDirection, out RaycastHit hitTp, 100f, wallsPlusTpMask)){
            // check if hit layer is tpMask
            if((tpMask & 1 << hitTp.collider.gameObject.layer) == 0){
                //print($"Hit {hitTp.collider.gameObject.layer} instead of {tpMask}");
                return;
            }
            ui.pointingCrosshair = true;
            if(Input.GetKeyDown(KeyCode.E)){
                /*print($"Setting pos from {transform.position} to {hitTp.point}");
                print(gameObject);*/
                GetComponent<PlayerMovement>().TeleportPlayer(hitTp.point);
                Physics.SyncTransforms();
                hasTp = false;
                ui.hasItem[2] = false;
            }
        }
        if(Physics.Raycast(transform.position + eyesOffset, cameraController.lookDirection, out RaycastHit hitDoor, interactDistance, doorMask)){
            print("Highlighted door");
            ui.highlightedCrosshair = true;
            if(Input.GetKeyDown(KeyCode.E)){
                ui.Finish();
            }
        }
    }
}
