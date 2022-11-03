using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

public class Visualizer : MonoBehaviour
{
    public GameObject quadPrefab;
    private const int samles = 32;
    GameObject[] rightQuads = new GameObject[samles];
    GameObject[] leftQuads = new GameObject[samles];
    private AudioData audioData;
    public float width = 0.1f, height = 0.5f, distance = 0.2f, amp = 0.5f;
    public bool mainMenu = true;
	public bool jobs;

    // Start is called before the first frame update
    void Start()
    {
        audioData = GameObject.Find("AudioData").GetComponent<AudioData>();

        //Spawn waves visual
        if (mainMenu)
        {	
            for (int i = 0; i < samles; i++)
            {				
                GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.parent = this.transform;
                instanceQuad.name = "Quad" + i;
                instanceQuad.transform.position = new Vector3(transform.position.x + distance * i, transform.position.y,
                    transform.position.z);
                instanceQuad.transform.rotation = transform.rotation;
                rightQuads[i] = instanceQuad;				
            }

            for (int i = 0; i < samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.parent = this.transform;
                instanceQuad.name = "mQuad" + i;
                instanceQuad.transform.position = new Vector3(transform.position.x - distance * i, transform.position.y,
                    transform.position.z);
                instanceQuad.transform.rotation = transform.rotation;
                leftQuads[i] = instanceQuad;
            }
            transform.localScale = Vector3.zero;
        }
        else
        {
            this.Wait(2, () => 
            {
                float angle = 36f / (float)samles;
                for (int i = 0; i < samles; i++)
                {
                    GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                    instanceQuad.transform.parent = this.transform;
                    instanceQuad.name = "Quad" + i;

                    transform.rotation = Quaternion.Euler(0, 0, 1 * ((i + 1) * angle));
                    instanceQuad.transform.position = new Vector3(transform.position.x + distance * i, transform.position.y,
                       transform.position.z);
                    instanceQuad.transform.rotation = Quaternion.Euler(transform.rotation.x,
                        transform.rotation.y, -1 * ((i + 1) * angle));

                    rightQuads[i] = instanceQuad;                    
                }
            });
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Animate audio waves
        if (mainMenu)
        {
			if (jobs)
			{
				NativeArray<float3> localScaleArrayRight = new NativeArray<float3>(samles, Allocator.TempJob);
				NativeArray<float3> localScaleArrayLeft = new NativeArray<float3>(samles, Allocator.TempJob);
				NativeArray<float> samplesArray = new NativeArray<float>(samles, Allocator.TempJob);
				
				for (int i = 0; i < samles; i++)
				{
					localScaleArrayRight[i] = rightQuads[i].transform.localScale;
					localScaleArrayLeft[i] = leftQuads[i].transform.localScale;
					samplesArray[i] = audioData.samples[i];
				}
				
				VisualizerJob visualizerJob = new VisualizerJob
				{
					localScaleArrayRight = localScaleArrayRight,
					localScaleArrayLeft = localScaleArrayLeft,
					samplesArray = samplesArray,
					width = width,
					height = height,
					amp = amp,
				};
				
				JobHandle jobHandle = visualizerJob.Schedule(samles, 8);
				jobHandle.Complete();
				
				for (int i = 0; i < samles; i++)
				{
					rightQuads[i].transform.localScale = localScaleArrayRight[i];
					leftQuads[i].transform.localScale = localScaleArrayLeft[i];
					audioData.samples[i] = samplesArray[i];
				}
				
				localScaleArrayRight.Dispose();
				localScaleArrayLeft.Dispose();
				samplesArray.Dispose();				
			}
			else
			{
				AnimateQuad(rightQuads);
				AnimateQuad(leftQuads);		
			}
        }
        else
        {
            if (Time.time > 2)
            {
                for (int i = 0; i < samles; i++)
                {                                                                                                   
                    rightQuads[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp * (i / 2 + 1) + height, 1);
                }
            }
        }
    }
	
	private void AnimateQuad(GameObject[] animQuads)
	{
		for (int i = 0; i < samles; i++)
        {                                                                                                    
            animQuads[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp + height, 1);
        }
	}
}

[BurstCompile]
public struct VisualizerJob : IJobParallelFor
{
	public NativeArray<float3> localScaleArrayRight;
	public NativeArray<float3> localScaleArrayLeft;
	public NativeArray<float> samplesArray;
	public float width, height, amp;
	public void Execute(int i)
	{
		localScaleArrayRight[i]= new float3(width, samplesArray[i] * amp + height, 1);
		localScaleArrayLeft[i]= new float3(width, samplesArray[i] * amp + height, 1);
	}
}
