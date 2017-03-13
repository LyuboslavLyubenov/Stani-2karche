namespace Assets.UITween.Editor
{

    using System.Collections;

    using Assets.UITween.Scripts;

    using UnityEditor;

    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(ReferencedFrom))] 
    public class ReferencedProxy : Editor
    {
        public override void OnInspectorGUI ()
        {
            this.DrawDefaultInspector ();
            EditorGUILayout.HelpBox ("This object is an UI Object", MessageType.Info);
        }
    }

    [CustomEditor(typeof(EasyTween))]
    public class EditorUITween : Editor
    {
        private bool positionEnabled;
        private bool scaleEnabled;
        private bool rotationEnabled;
        EasyTween tweenScript;

        public void OnEnable ()
        {
            this.tweenScript = ((EasyTween)this.target);
        }

        public void OnSceneGUI ()
        {
            if (this.tweenScript != null) {
                if (this.tweenScript.rectTransform) {
                    Handles.color = new Color (1f, 0f, 0f, 0.5f);
                    Handles.DrawSolidDisc (this.tweenScript.rectTransform.position, Vector3.forward, 10f);
                    Handles.color = Color.cyan;
                }
            }
        }

        public override void OnInspectorGUI ()
        {
            EditorGUILayout.BeginVertical();

            this.DrawDefaultInspector ();

            if (this.tweenScript != null) {
                if (this.tweenScript.rectTransform) {
                    if (!Application.isPlaying) {
                        this.tweenScript.animationParts.SetAniamtioDuration (EditorGUILayout.Slider ("Animation Duration (Sec)", this.tweenScript.animationParts.GetAnimationDuration (), 0.01f, 10f));

                        this.EditorFade ();
                        this.EditorPos ();
                        this.EditorRot ();
                        this.EditorScale ();
                        this.GetButtonPos ();
					
                        if (!this.tweenScript.rectTransform.gameObject.GetComponent<ReferencedFrom> ()) {
                            this.tweenScript.rectTransform.gameObject.AddComponent<ReferencedFrom> ();
                        }
					
                        if (GUI.changed) {
                            EditorUtility.SetDirty (this.tweenScript);
                        }
                    } else {
                        if (GUILayout.Button ("Animate")) {
                            this.tweenScript.OpenCloseObjectAnimation ();
                        }
                        EditorGUILayout.HelpBox ("Editor Not Available in Play Mode", MessageType.Info);
                    }
                } else {
                    EditorGUILayout.HelpBox ("Please set \"Rect Trasnform Variable\" that contains \"RectTransform\" component. UI Components", MessageType.Info);
                }
            }

            EditorGUILayout.EndVertical();
        }

        void GetAniamButtons ()
        {
            if (this.positionEnabled || this.rotationEnabled || this.scaleEnabled || this.tweenScript.animationParts.FadePropetiesAnim.IsFadeEnabled ()) {
                if (GUILayout.Button ("Animate")) {
                    this.tweenScript.OpenCloseObjectAnimation ();
                }
            }
        }

        void GetButtonPos ()
        {
            EditorGUILayout.BeginHorizontal ();
            if (this.positionEnabled || this.rotationEnabled || this.scaleEnabled) {
                if (GUILayout.Button ("Get Start Values")) {
                    this.GetStartValues ();
                }
			
                if (GUILayout.Button ("Get End Values")) {
                    this.GetEndValues ();
                }
            }
            EditorGUILayout.EndHorizontal ();
            EditorGUILayout.Space ();
		
            this.GetAniamButtons ();
		
            EditorGUILayout.Space ();
            EditorGUILayout.BeginHorizontal ();
            if (this.positionEnabled || this.rotationEnabled || this.scaleEnabled || this.tweenScript.animationParts.FadePropetiesAnim.IsFadeEnabled ()) {
                if (GUILayout.Button ("Set To Start Values")) {
                    this.SetStartValues ();
                }
			
                if (GUILayout.Button ("Set To End Values")) {
                    this.SetEndValues ();
                }
            }
            EditorGUILayout.EndHorizontal ();
        }

        void GetStartValues ()
        {
            RectTransform selectedTransform = this.tweenScript.rectTransform;
		
            this.tweenScript.animationParts.PositionPropetiesAnim.SetPosStart ((Vector3)selectedTransform.anchoredPosition, selectedTransform);
            this.tweenScript.animationParts.ScalePropetiesAnim.StartScale = selectedTransform.localScale;
            this.tweenScript.animationParts.RotationPropetiesAnim.StartRot = selectedTransform.localEulerAngles;
        }

        void GetEndValues ()
        {
            RectTransform selectedTransform = this.tweenScript.rectTransform;
		
            this.tweenScript.animationParts.PositionPropetiesAnim.SetPosEnd ((Vector3)selectedTransform.anchoredPosition, selectedTransform.transform);
            this.tweenScript.animationParts.ScalePropetiesAnim.EndScale = selectedTransform.localScale;
            this.tweenScript.animationParts.RotationPropetiesAnim.EndRot = selectedTransform.localEulerAngles;
        }

        void SetStartValues ()
        {
            RectTransform selectedTransform = this.tweenScript.rectTransform;
		
            if (this.tweenScript.animationParts.PositionPropetiesAnim.IsPositionEnabled ())
                selectedTransform.anchoredPosition = (Vector2)this.tweenScript.animationParts.PositionPropetiesAnim.StartPos;
		
            if (this.tweenScript.animationParts.ScalePropetiesAnim.IsScaleEnabled ())
                selectedTransform.localScale = this.tweenScript.animationParts.ScalePropetiesAnim.StartScale; 
		
            if (this.tweenScript.animationParts.RotationPropetiesAnim.IsRotationEnabled ())
                selectedTransform.localEulerAngles = this.tweenScript.animationParts.RotationPropetiesAnim.StartRot;
		
            if (this.tweenScript.animationParts.FadePropetiesAnim.IsFadeEnabled ()) {
                if (this.tweenScript.IsObjectOpened ())
                    this.SetAlphaValue (selectedTransform.transform, this.tweenScript.animationParts.FadePropetiesAnim.GetEndFadeValue ());
                else
                    this.SetAlphaValue (selectedTransform.transform, this.tweenScript.animationParts.FadePropetiesAnim.GetStartFadeValue ());
            }
        }

        void SetEndValues ()
        {
            RectTransform selectedTransform = this.tweenScript.rectTransform;
		
            if (this.tweenScript.animationParts.PositionPropetiesAnim.IsPositionEnabled ())
                selectedTransform.anchoredPosition = (Vector2)this.tweenScript.animationParts.PositionPropetiesAnim.EndPos;
		
            if (this.tweenScript.animationParts.ScalePropetiesAnim.IsScaleEnabled ())
                selectedTransform.localScale = this.tweenScript.animationParts.ScalePropetiesAnim.EndScale; 
		
            if (this.tweenScript.animationParts.RotationPropetiesAnim.IsRotationEnabled ())
                selectedTransform.localEulerAngles = this.tweenScript.animationParts.RotationPropetiesAnim.EndRot;
		
            if (this.tweenScript.animationParts.FadePropetiesAnim.IsFadeEnabled ()) {
                if (this.tweenScript.IsObjectOpened ())
                    this.SetAlphaValue (selectedTransform.transform, this.tweenScript.animationParts.FadePropetiesAnim.GetStartFadeValue ());
                else
                    this.SetAlphaValue (selectedTransform.transform, this.tweenScript.animationParts.FadePropetiesAnim.GetEndFadeValue ());
            }
        }

        void SetAlphaValue (Transform _objectToSetAlpha, float alphaValue)
        {
            if (_objectToSetAlpha.GetComponent<MaskableGraphic> ()) {
                MaskableGraphic GraphicElement = _objectToSetAlpha.GetComponent<MaskableGraphic> ();
			
                Color objectColor = GraphicElement.color;
			
                objectColor.a = alphaValue;
                GraphicElement.color = objectColor;
            }
		
            if (_objectToSetAlpha.childCount > 0) {
                for (int i = 0; i < _objectToSetAlpha.childCount; i++) {
                    if (!_objectToSetAlpha.GetChild (i).GetComponent<ReferencedFrom> () || this.tweenScript.animationParts.FadePropetiesAnim.IsFadeOverrideEnabled ()) {
                        this.SetAlphaValue (_objectToSetAlpha.GetChild (i), alphaValue);
                    }
                }
            }
        }

        void EditorFade ()
        {
            this.tweenScript.animationParts.FadePropetiesAnim.SetFadeEnable (EditorGUILayout.BeginToggleGroup ("Fade In & Out",
                this.tweenScript.animationParts.FadePropetiesAnim.IsFadeEnabled ()));
	

            if (this.tweenScript.animationParts.FadePropetiesAnim.IsFadeEnabled ()) {
                EditorGUILayout.LabelField ("Fade Start and End Values");

                EditorGUILayout.BeginHorizontal ();

                float fadeValueStart = EditorGUILayout.FloatField ("Start Value", this.tweenScript.animationParts.FadePropetiesAnim.GetStartFadeValue ());
                float fadeValueEnd = EditorGUILayout.FloatField ("End Value", this.tweenScript.animationParts.FadePropetiesAnim.GetEndFadeValue ());

                this.tweenScript.animationParts.FadePropetiesAnim.SetFadeValues (fadeValueStart, fadeValueEnd);

                EditorGUILayout.EndHorizontal ();
            
                this.tweenScript.animationParts.FadePropetiesAnim.SetFadeOverride (EditorGUILayout.BeginToggleGroup ("Fade Override", 
                    this.tweenScript.animationParts.FadePropetiesAnim.IsFadeOverrideEnabled ()));
                     
                EditorGUILayout.EndToggleGroup ();
            }

            EditorGUILayout.EndToggleGroup ();
        }

        void EditorPos ()
        {
            this.tweenScript.animationParts.PositionPropetiesAnim.SetPositionEnable (EditorGUILayout.BeginToggleGroup ("Position Animation", 
                this.tweenScript.animationParts.PositionPropetiesAnim.IsPositionEnabled ()));
            this.positionEnabled = this.tweenScript.animationParts.PositionPropetiesAnim.IsPositionEnabled ();
		
            if (this.positionEnabled) {
                this.tweenScript.animationParts.PositionPropetiesAnim.SetPosStart (EditorGUILayout.Vector3Field ("Start Move", this.tweenScript.animationParts.PositionPropetiesAnim.StartPos), this.tweenScript.rectTransform);
                this.tweenScript.animationParts.PositionPropetiesAnim.SetPosEnd (EditorGUILayout.Vector3Field ("End Move", this.tweenScript.animationParts.PositionPropetiesAnim.EndPos), this.tweenScript.rectTransform.transform);
			
                EditorGUILayout.BeginHorizontal ();

                if (this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveEnterPos == null) {
                    this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveEnterPos = new AnimationCurve ();
                }
				
                if (this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveExitPos == null) {
                    this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveExitPos = new AnimationCurve ();
                }
				
                this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveEnterPos = EditorGUILayout.CurveField ("Start Tween Move", 
                    this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveEnterPos);
                EditorGUILayout.Space ();
                this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveExitPos = EditorGUILayout.CurveField ("Exit Tween Move", 
                    this.tweenScript.animationParts.PositionPropetiesAnim.TweenCurveExitPos);

                EditorGUILayout.EndHorizontal ();
			
                EditorGUILayout.Space ();
            }
		
            EditorGUILayout.EndToggleGroup ();
        }

        void EditorScale ()
        {
            this.tweenScript.animationParts.ScalePropetiesAnim.SetScaleEnable (EditorGUILayout.BeginToggleGroup ("Scale Animation", 
                this.tweenScript.animationParts.ScalePropetiesAnim.IsScaleEnabled ()));
            this.scaleEnabled = this.tweenScript.animationParts.ScalePropetiesAnim.IsScaleEnabled ();
		
            if (this.scaleEnabled) {
                this.tweenScript.animationParts.ScalePropetiesAnim.StartScale = EditorGUILayout.Vector3Field ("Start Scale", this.tweenScript.animationParts.ScalePropetiesAnim.StartScale);
                this.tweenScript.animationParts.ScalePropetiesAnim.EndScale = EditorGUILayout.Vector3Field ("End Scale", this.tweenScript.animationParts.ScalePropetiesAnim.EndScale);
			
                EditorGUILayout.BeginHorizontal ();

                if (this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveEnterScale == null) {
                    this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveEnterScale = new AnimationCurve ();
                }
				
                if (this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveExitScale == null) {
                    this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveExitScale = new AnimationCurve ();
                }
				
                this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveEnterScale = EditorGUILayout.CurveField ("Start Tween Scale", 
                    this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveEnterScale);
                EditorGUILayout.Space ();
                this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveExitScale = EditorGUILayout.CurveField ("Exit Tween Scale", 
                    this.tweenScript.animationParts.ScalePropetiesAnim.TweenCurveExitScale);

                EditorGUILayout.EndHorizontal ();
			
                EditorGUILayout.Space ();
            }
		
            EditorGUILayout.EndToggleGroup ();
        }

        void EditorRot ()
        {
            this.tweenScript.animationParts.RotationPropetiesAnim.SetRotationEnable (EditorGUILayout.BeginToggleGroup ("Rotation Animation", 
                this.tweenScript.animationParts.RotationPropetiesAnim.IsRotationEnabled ()));
            this.rotationEnabled = this.tweenScript.animationParts.RotationPropetiesAnim.IsRotationEnabled ();
		
            if (this.rotationEnabled) {
                this.tweenScript.animationParts.RotationPropetiesAnim.StartRot = EditorGUILayout.Vector3Field ("Start Rotation", this.tweenScript.animationParts.RotationPropetiesAnim.StartRot);
                this.tweenScript.animationParts.RotationPropetiesAnim.EndRot = EditorGUILayout.Vector3Field ("End Rotation", this.tweenScript.animationParts.RotationPropetiesAnim.EndRot);
			
                EditorGUILayout.BeginHorizontal ();

                if (this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveEnterRot == null) {
                    this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveEnterRot = new AnimationCurve ();
                }
				
                if (this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveExitRot == null) {
                    this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveExitRot = new AnimationCurve ();
                }
				
                this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveEnterRot = EditorGUILayout.CurveField ("Start Tween Rotation", 
                    this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveEnterRot);
                EditorGUILayout.Space ();
                this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveExitRot = EditorGUILayout.CurveField ("Exit Tween Rotation", 
                    this.tweenScript.animationParts.RotationPropetiesAnim.TweenCurveExitRot);

                EditorGUILayout.EndHorizontal ();
			
                EditorGUILayout.Space ();
            }
		
            EditorGUILayout.EndToggleGroup ();
        }
    }

}