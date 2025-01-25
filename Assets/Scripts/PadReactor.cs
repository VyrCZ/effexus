using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PadReactor : MonoBehaviour {

    public class EffectObject{
        public Effect effect;
        public float duration;
        public GameObject UIObject;
    }
    
    public enum Effect{
        Speed,
        Jump,
        Hover,
        NoJump,
        Timewarp
    }
    public List<EffectObject> currentEffects = new();
    public GameObject UIEffectsContainer;
    public GameObject UIEffectPrefab; // Children: Name, Sprite, Slider (duration)
    // this is on the player and handles how the player reacts to the pads
    [Header("Effect Values")]
    public float effectMoveSpeed = 15f;
    public float effectJumpHeight = 10f;

    private float originalMoveSpeed;
    private float originalJumpHeight;
    [Header("Effect Durations")]
    public float speedEffectDuration = 10f;
    public float jumpEffectDuration = 10f;
    public float hoverEffectDuration = 10f;
    public float timewarpEffectDuration = 10f;
    public Vector3? warpPos = null;
    public bool justWarped = false;
    public bool activeWarp = false;

    void Update(){
        List<EffectObject> effectsToRemove = new();
        if(currentEffects.Count > 0) 
            print("=============================");
        foreach(EffectObject effectObject in currentEffects){
            if(effectObject.duration > 0) 
                print($"{effectObject.effect} effect remaining: {effectObject.duration}");

            effectObject.duration -= Time.deltaTime;
            effectObject.UIObject.transform.GetChild(2).GetComponent<Slider>().value = effectObject.duration;
            if(effectObject.duration <= 0){
                effectsToRemove.Add(effectObject);
            }
        }

        foreach(EffectObject effectObject in effectsToRemove){

            RemoveEffect(effectObject);
        }
        //print(warpPos);
    }

    private void RemoveEffect(EffectObject effectObject){
        switch(effectObject.effect){
            case Effect.Speed:
                GetComponent<PlayerMovement>().moveSpeed = originalMoveSpeed;
                GetComponent<PlayerMovement>().moveSpeed = originalMoveSpeed;
                break;
            case Effect.Jump:
                GetComponent<PlayerMovement>().jumpHeight = originalJumpHeight;
                break;
            case Effect.Hover:
                GetComponent<PlayerMovement>().isHovering = false;
                break;
            case Effect.NoJump:
                GetComponent<PlayerMovement>().canJump = true;
                break;
            case Effect.Timewarp:
                GetComponent<PlayerMovement>().TeleportPlayer(warpPos.Value);
                activeWarp = false;
                justWarped = true;
                Physics.SyncTransforms();
                break;
        }
        Destroy(effectObject.UIObject);
        currentEffects.Remove(effectObject);
    }

    private void AddEffect(Effect effect, float duration){
        foreach(EffectObject testedObject in currentEffects){
            if(testedObject.effect == effect){
                testedObject.duration = duration;
                return;
            }
        }
        EffectObject effectObject = new()
        {
            effect = effect,
            duration = duration
        };
        currentEffects.Add(effectObject);

        GameObject effectUI = Instantiate(UIEffectPrefab, UIEffectsContainer.transform);
        effectUI.transform.GetChild(2).GetComponent<Slider>().maxValue = duration;

        switch(effect){
            case Effect.Speed:
                originalMoveSpeed = GetComponent<PlayerMovement>().moveSpeed;
                originalMoveSpeed = GetComponent<PlayerMovement>().moveSpeed;
                GetComponent<PlayerMovement>().moveSpeed = effectMoveSpeed;
                GetComponent<PlayerMovement>().moveSpeed = effectMoveSpeed;
                effectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Speed";
                effectUI.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Effects/Speed");
                break;
            case Effect.Jump:
                originalJumpHeight = GetComponent<PlayerMovement>().jumpHeight;
                GetComponent<PlayerMovement>().jumpHeight = effectJumpHeight;
                effectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Jump";
                effectUI.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Effects/Jump");
                break;
            case Effect.Hover:
                GetComponent<PlayerMovement>().isHovering = true;
                effectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Hover";
                effectUI.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Effects/Hover");
                break;
            case Effect.NoJump:
                GetComponent<PlayerMovement>().canJump = false;
                effectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "No Jump";
                effectUI.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Effects/NoJump");
                break;
            case Effect.Timewarp:
                warpPos = transform.position;
                activeWarp = true;
                Debug.LogWarning(warpPos);
                effectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Timewarp";
                effectUI.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Effects/Timewarp");
                break;
        }
        effectObject.UIObject = effectUI;
    }

    void Die(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("PowerPad")){
            var type = other.gameObject.GetComponent<PowerPad>().padType;
            switch(type){
                case PadType.Speed:
                    AddEffect(Effect.Speed, speedEffectDuration);
                    break;
                case PadType.Jump:
                    AddEffect(Effect.Jump, jumpEffectDuration);
                    break;
                case PadType.Hover:
                    AddEffect(Effect.Hover, hoverEffectDuration);
                    break;
                case PadType.NoJump:
                    AddEffect(Effect.NoJump, -1);
                    break;
                case PadType.Timewarp:
                    if(!justWarped){
                        AddEffect(Effect.Timewarp, timewarpEffectDuration);
                    }
                    break;
            }
        }
        else if(other.CompareTag("ResetField")){
            foreach(EffectObject effectObject in new List<EffectObject>(currentEffects)){
                RemoveEffect(effectObject);
            }
        }
        else if(other.CompareTag("Hazard")){
            Die();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("PowerPad")){
            if(other.gameObject.GetComponent<PowerPad>().padType == PadType.Timewarp){
                justWarped = false;
            }
        }
    }
}
