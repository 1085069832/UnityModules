﻿using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace Leap.Unity{
  
  /**
  * The Transition class animates the position, rotation, scale, and color of child game objects.
  * Use a Transition to animate hand attachments when they turn on or off. For example, you could
  * make an arm HUD rotate or fade into view when activated.
  *
  * The Transition component should be placed on an empty game object that is a child of one of the hand 
  * attachment transforms (i.e. for the palm, one of the fingers, etc.). The attached objects to be affected 
  * by the transition should be children of that empty game object.
  *
  * Assign the Transition component to the AttachmentController.Transition property. The AttachmentController 
  * will then call the Transition methods at the appropriate times.
  * 
  * The Transition component uses AnimationCurve objects to control the animation. The curves you use should cover a 
  * domain of -1 to 1. The portion of the curve from -1 to 0 is used for the "in" transition (off to on state); the portion
  * from 0 to 1 is used for the "out" transition (on to off state).
  *
  * You can use the Simulate slider in the Unity inspector to observe the affect of the transition in the editor.
  * @since 4.1.4
  */
  [ExecuteInEditMode]
  public class Transition : MonoBehaviour {

    /**
    * Specifies whether to animate position.
    * The position of the Transition game object is animated. Any child objects maintain their
    * respective local positions relative to the transition object.
    * @since 4.1.3
    */
    [Tooltip("Whether to animate position")]
    public bool AnimatePosition = false;

    /**
    * The position of the transition object in the fully transitioned (out) state.
    * @since 4.1.3
    */
    [Tooltip("Position before in transition and after an out transition")]
    public Vector3 OutPosition = Vector3.zero;

    /**
    * The curve controlling position.
    * A curve value of 1 is fully transitioned off. A curve value of 0 is the on state (the transition settings have no influence on position).
    * @since 4.1.3
    */
    [Tooltip("Easing curve for position transitions. [-1,0] is in transition; [0,+1] is out transition.")]
    public AnimationCurve PositionCurve = new AnimationCurve(new Keyframe(-1,1), new Keyframe(0,0), new Keyframe(1,1));

    /**
    * Specifies whether to animate rotation.
    * The rotation of the Transition game object is animated. Any child objects maintain their
    * respective local rotations relative to the transition object.
    * @since 4.1.3
    */
    [Tooltip("Whether to animate rotation")]
    public bool AnimateRotation = false;

    /**
    * The rotation of the transition object in the fully transitioned (out) state.
    * @since 4.1.3
    */
    [Tooltip("Rotation before an in transition and after an out transition.")]
    public Quaternion OutRotation = Quaternion.identity;

    /**
    * The curve controlling rotation.
    * A curve value of 1 is fully transitioned off. A curve value of 0 is the on state (the transition settings have no influence on rotation).
    * @since 4.1.3
    */
    [Tooltip("Easing curve for rotation transitions. [-1,0] is in transition; [0,+1] is out transition.")]
    public AnimationCurve RotationCurve = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));

    /**
    * Specifies whether to animate scale.
    * The scale of the Transition game object is animated. Any child objects maintain their
    * respective local scale relative to the transition object.
    * @since 4.1.3
    */
    [Tooltip("Whether to animate scale.")]
    public bool AnimateScale = false;

    /**
    * The scale of the transition object in the fully transitioned (out) state.
    * @since 4.1.3
    */
    [Tooltip("Scale before an in transition and after an out transition.")]
    public Vector3 OutScale = Vector3.one;

    /**
    * The curve controlling scale.
    * A curve value of 1 is fully transitioned off. A curve value of 0 is the on state (the transition settings have no influence on scale).
    * @since 4.1.3
    */
    [Tooltip("Easing curve for scale transitions. [-1,0] is in transition; [0,+1] is out transition.")]
    public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));

    /**
    * Specifies whether to animate color.
    * The color of each child object (with a Renderer component) is modified individually.
    * Child objects must use a shader that uses the property specified by ColorShaderPropertyName.
    * To change the alpha color component, the shader must also support alpha blending. The standard
    * Unity shaders have a Rendering Mode property that must be set to Transparency or Fade in order for
    * changes in alpha to be rendered. Other shaders may have their own requirements.
    * @since 4.1.3
    */
    [Tooltip("Whether to animate color")]
    public bool AnimateColor = false;

    /**
    * The name of the shader property controlling object color.
    * The default value is the name used by the standard Unity shaders. You can view the names used by a shader
    * by finding the Shader component in the inspector panel for the owning game object and selecting the 
    * "Edit Shader..." command from the shader component's drop-down menu.
    * 
    * All objects whose color you wish to animate must use the same color name. 
    * @since 4.1.3
    */
    [Tooltip("The name of the shader property controlling object color")]
    public string ColorShaderPropertyName = "_Color";

    /**
    * The color of the transition object in the fully transitioned (out) state.
    * The color is applied to the shader property specified by ColorShaderPropertyName variable.
    * @since 4.1.3
    */
    [Tooltip("Color before an in transition and after an out transition.")]
    public Color OutColor = Color.black;

    /**
    * The curve controlling color.
    * A curve value of 1 is fully transitioned off. A curve value of 0 is the on state (the transition settings have no influence on color).
    * @since 4.1.3
    */
    [Tooltip("Easing curve for color transitions. [-1,0] is in transition; [0,+1] is out transition.")]
    public AnimationCurve ColorCurve = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));

    /**
    * The duration of the transition in seconds.
    * @since 4.1.3
    */
    [Tooltip("Length of transition in seconds")]
    [Range(.001f, 2.0f)]
    public float Duration = 0.5f; //seconds

    /**
    * A value used in edit mode to simulate the transition.
    * Change the value to observe how the transition will look when played.
    * @since 4.1.3
    */
    [Tooltip("Simulates transition in edit mode")]
    [Range (-1, 1)]
    public float Simulate = 0.0f;
    
    private float progress = 0.0f;
    private MaterialPropertyBlock materialProperties;
    private Vector3 localPosition;
    private Quaternion localRotation;
    private Vector3 localScale;

    /**
    * Dispatched when a transition is complete.
    * @since 4.1.3
    */
    [Tooltip("Dispatched when a transition is finished")]
    public UnityEvent OnComplete;

    /**
    * Dispatched for each animation step before the transition does its own update.
    * The event parameter is a float between -1 and 0 for an in transtion and between 0 and 1
    * for an out transition.
    * @since 4.1.4
    */
    [Tooltip("Dispatched each frame during a transition")]
    public AnimationStepEvent OnAnimationStep;

#if UNITY_EDITOR
    private void Reset() {
      captureInitialState();
    }

    private void Update() {
      if (!EditorApplication.isPlaying) {
        updateTransition(Simulate);
      }
    }
  #endif
  
    private void Awake(){
      materialProperties = new MaterialPropertyBlock();
      captureInitialState();
      updateTransition(0.0f);
    }

    /**
    * Play the transition from off to on.
    * @since 4.1.3
    */
    public void TransitionIn(){
      if (isActiveAndEnabled) {
        StopAllCoroutines();
        StartCoroutine(transitionIn());
      }
    }

    /**
    * Play the transition from on to off.
    * @since 4.1.3
    */
    public void TransitionOut(){
      if (isActiveAndEnabled) {
        StopAllCoroutines();
        StartCoroutine(transitionOut());
      }
    }

    /**
    * Saves the local transform values to use as the on state.
    * @since 4.1.3
    */
    protected virtual void captureInitialState() {
      localPosition = transform.localPosition;
      localRotation = transform.localRotation;
      localScale = transform.localScale;
    }

    /**
    * Sets the transition directly to the on state without any animation.
    * @since 4.1.3
    */
    public virtual void GotoOnState() {
      transform.localPosition = localPosition;
      transform.localRotation = localRotation;
      transform.localScale = localScale;
    }

    /**
    * A coroutine that updates the transition state when playing the "in" transition.
    * @since 4.1.3
    */
    protected IEnumerator transitionIn(){
      float start = Time.time;
      do {
        progress = (Time.time - start)/Duration;
        OnAnimationStep.Invoke(progress - 1);
        updateTransition(progress - 1);
        yield return null;
      } while(progress <= 1);
      progress = 0;
      OnComplete.Invoke();
    }

    /**
    * A coroutine that updates the transition state when playing the "out" transition.
    * @since 4.1.3
    */
    protected IEnumerator transitionOut(){
      float start = Time.time;
      do {
        progress = (Time.time - start)/Duration;
        OnAnimationStep.Invoke(progress);
        updateTransition(progress);
        yield return null;
      } while(progress <= 1);
      progress = 0;
      OnComplete.Invoke();
    }

    /**
    * Calls the individual animation functions.
    * @since 4.1.3
    */
    protected virtual void updateTransition(float interpolationPoint){
      if (AnimatePosition) doAnimatePosition(interpolationPoint);
      if (AnimateRotation) doAnimateRotation(interpolationPoint);
      if (AnimateScale) doAnimateScale(interpolationPoint);
      if (AnimateColor) doAnimateColor(interpolationPoint);
    }

    /**
    * Lerps between the in and out positions based on the animation curve.
    * @since 4.1.3
    */
    protected virtual void doAnimatePosition(float interpolationPoint) {
      transform.localPosition = Vector3.Lerp(localPosition, OutPosition, PositionCurve.Evaluate(interpolationPoint));
    }

    /**
    * Lerps between the in and out rotations based on the animation curve.
    * @since 4.1.3
    */
    protected virtual void doAnimateRotation(float interpolationPoint) {
      transform.localRotation = Quaternion.Lerp(localRotation, OutRotation, RotationCurve.Evaluate(interpolationPoint));
    }

    /**
    * Lerps between the in and out scales based on the animation curve.
    * @since 4.1.3
    */
    protected virtual void doAnimateScale(float interpolationPoint) {
      transform.localScale = Vector3.Lerp(localScale, OutScale, ScaleCurve.Evaluate(interpolationPoint));
    }

    /**
    * For all child objects, lerps between the in and out colors based on the animation curve.
    * @since 4.1.3
    */
    protected virtual void doAnimateColor(float interpolationPoint) {
      float influence = ColorCurve.Evaluate(interpolationPoint);
      Transform[] children = GetComponentsInChildren<Transform>(true);
      for (int g = 0; g < children.Length; g++) {
        Renderer renderer = children[g].gameObject.GetComponent<Renderer>();
        if (renderer != null) {
          materialProperties = new MaterialPropertyBlock();
          renderer.GetPropertyBlock(materialProperties);
          materialProperties.SetColor(ColorShaderPropertyName, Color.Lerp(renderer.sharedMaterial.color, OutColor, influence));
          renderer.SetPropertyBlock(materialProperties);
        }
      }
    }

  }

  /**
   * An event class that is dispatched by a Transition for each animation
   * step during a transition. The event occurs once per frame for the duration of
   * a transition.
   * The event parameter provides the current interpolation value between -1 and 0 for an
   * in transition and between 0 and +1 for an out transition..
   * @since 4.1.4
   */
  [System.Serializable]
  public class AnimationStepEvent : UnityEvent<float> { }
}