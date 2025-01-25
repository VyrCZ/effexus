using UnityEngine;

public class Elevator : MonoBehaviour {
    public bool goingUp = true;
    public bool active = false;
    public Material disabledMaterial;
    public Material enabledMaterial;
    MeshRenderer meshRenderer;

    public float teleportOffset = 5f; // how much do you teleport up or down

    private void Start() {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        if(active){
            Activate();
        }
        else{
            var mats = meshRenderer.materials;
            mats[0] = disabledMaterial;
            meshRenderer.materials = mats;
        }
    }

    public void Activate()
    {
        active = true;
        var mats = meshRenderer.materials;
        mats[0] = enabledMaterial;
        meshRenderer.materials = mats;
    }

    void Teleport(Transform player){
        print("Teleporting up");
        player.GetComponent<PlayerMovement>().TeleportPlayer(player.position + (goingUp ? 1 : -1) * teleportOffset * Vector3.up);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            if(active){
                Teleport(other.transform);
            }
        }
    }

    private void OnDrawGizmos() {
        if(goingUp){
            Gizmos.color = Color.red;
        }
        else{
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawSphere(transform.position + (goingUp ? Vector3.up : Vector3.down) * teleportOffset, 0.25f);
    }
}