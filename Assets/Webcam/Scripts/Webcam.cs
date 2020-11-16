﻿/*
	Detecção de movimento com Webcam
	Desenvolvido por Murillo Brandão
	Sob orientação do Prof. Dr. Luciano Vieira de Araújo
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Webcam : MonoBehaviour {

	/*
		PARAMETROS
		Display - RawImage para exibir o blend
		threshold - sensibilidade da detecção
		blendColor - cor de exibição da detecção
	 */
	public RawImage Display;
	[SerializeField]
	[Range(0,255)]
	int rMin = 150,rMax=255;
	[SerializeField]
	[Range(0,255)]
	int gMax = 100,gMin=0;
	[SerializeField]
	[Range(0,255)]
	int bMax = 100,bMin=0;
	[SerializeField]
	float minDiff=1f;
	
	[Range(0, 255)]
	public int threshold = 100;
	[Range(0f, 1f)]
	public float density = .02f;

	public Color32 blendColor = new Color32(255,217, 20, 20);
	Vector3 rect;

	bool initialized = false;
	WebCamTexture webcamTexture;
	Texture2D blendTexture;
	int diffsum = 0;
	int nullCount = 0;
	int itCounter = 1;
	Color32[] lastData = null;
	Color32[] blendData = null;
	Color32[] checkData = null;

	[HideInInspector]
	public float scale = 1f;
	// Escala da webcam com relação ao display


	IEnumerator Start() {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);
        if( Application.HasUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone) ){
			StartWebcam();
        }

    }


	/*
		StartWebcam
		Inicializa a webcam e texturas
	 */
	void StartWebcam(){
		webcamTexture = new WebCamTexture(426, 240, 30);
		webcamTexture.requestedFPS = 30;
        webcamTexture.Play();
		blendTexture = new Texture2D( webcamTexture.width, webcamTexture.height );

		if( Display ){
			Display.texture = blendTexture;
			scale = webcamTexture.width / Display.rectTransform.rect.width;
		}

		initialized = true;
	}


	/*
		UPDATE
		Todo frame calcula a diferença de frames e atualiza a textura
	 */
	void Update()
    {
        // Aguarda inicialização e inicializa variaveis
		if (!initialized || webcamTexture.width == 0) return;
        if (lastData == null)
        {
            lastData = webcamTexture.GetPixels32();
            blendData = new Color32[lastData.Length];
			checkData = new Color32[ lastData.Length ];
            for (int i = 0; i < lastData.Length; i++)
            {
                blendData[i] = new Color32(0, 0, 0, 0);
				checkData[i] = new Color32(0, 0, 0, 0);
            }
        }
		Difference();
		 if (diffsum == 0)
        {
            if (nullCount++ < 5)  return;
        }
        else nullCount = 0;	
        


        // Atualiza textura e vetor de checagem
		for(int i=0; i<blendData.Length; i++){
			checkData[i] = blendData[i];
		}
		blendTexture.SetPixels32(blendData);
        blendTexture.Apply();
    }



    /*
		DIFFERENCE
		Calcula a diferença entre o frame atual e o frame anterior
		Coloca a diferença em blendData
	*/
    void Difference(){
		Color32[] actualData = webcamTexture.GetPixels32();
		diffsum = 0;	
		for(int i=0, len=actualData.Length; i<len; i++){
			int a = actualData[i].r - lastData[i].r;
			if( (a^a>>31)-(a>>31) > threshold && 
			(actualData[i].r>=rMin && actualData[i].g>=gMin && actualData[i].b>=bMin) &&
			(actualData[i].r<=rMax && actualData[i].g<=gMax && actualData[i].b<=bMax)){
				blendData[i] = blendColor;
				diffsum += 1;
			}
			else{
				blendData[i] = new Color(0, 0, 0, 0);
			}
		}
		lastData = actualData;
	}


	/*
		CHECKAREA
		Checa se há interação em uma região da webcam
		retorna true ou false
	 */
	public bool checkArea( int x, int y, int width, int height ){
		if( !initialized || checkData == null ) return false;
		
		int sum = 0;
		for(int i=0; i<width*height; i++){
			int tx = x + i%width;
				tx = webcamTexture.width - tx;
			int ty = y + (int)Mathf.Floor(i/width);
				ty = webcamTexture.height - ty;
			int p = (webcamTexture.width * ty) + tx;
			sum += (checkData[p].a > 0) ? 1 : 0;
		}
		float d = ((float) sum)/((float) (width*height));
		
		return ( d > density );
	}
}