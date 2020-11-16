﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class patternExample : MonoBehaviour
{
    [SerializeField]
    float interval=.5f;
    [SerializeField]
    bool play=false;
    [SerializeField]
    Color32 checkedColor=new Color32( 163, 255, 96, 255 );
    [SerializeField]
    Color32 uncheckedColor;
    [SerializeField]
    AudioClip pointSound;
    [SerializeField]
    AudioClip completionSound;
    [SerializeField]
    AudioClip errorSound;
    [SerializeField]
    GameObject checkedEffect;
    [SerializeField]
    Vector3 checkedOffset= Vector3.zero;
    int i=1;
    
    // Start is called before the first frame update
    void Awake()
    {   
        transform.parent.GetComponent<Pattern>().newPattern(checkedColor,uncheckedColor,pointSound,errorSound,completionSound,play,interval);
    }
    public AudioClip getPointSound(){
        return pointSound;
    }
    public GameObject getPointEffect(){
        return checkedEffect;
    }
    public Vector3 getEffectOffset(){
        return checkedOffset;
    }
    public Color32[] getColors(){
        Color32[] colors = {checkedColor,uncheckedColor};
        return colors;
    }
    // Update is called once per frame
    
}
