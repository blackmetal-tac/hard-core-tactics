using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

public class Visualizer : MonoBehaviour
{    
    [SerializeField] private GameObject _quadPrefab;
	[SerializeField] private float _width = 0.1f, _height = 0.5f, _distance = 0.2f, _amp = 0.5f;
    [SerializeField] private bool _jobs;
	private AudioData _audioData;
    private const int _samles = 32;
    private GameObject[] _rightQuads = new GameObject[_samles], _leftQuads = new GameObject[_samles];
    private string _sceneName;

    // Start is called before the first frame update
    void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _audioData = GameObject.Find("AudioData").GetComponent<AudioData>();

        //Spawn waves visual
        if (_sceneName == "MainMenu")
        {	
            for (int i = 0; i < _samles; i++)
            {				
                GameObject instanceQuad = (GameObject)Instantiate(_quadPrefab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.SetParent(this.transform);
                instanceQuad.name = "Quad" + i;
                instanceQuad.transform.position = new Vector3(transform.position.x + _distance * i, transform.position.y,
                    transform.position.z);
                instanceQuad.transform.rotation = transform.rotation;
                _rightQuads[i] = instanceQuad;				
            }

            for (int i = 0; i < _samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(_quadPrefab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.SetParent(this.transform);
                instanceQuad.name = "mQuad" + i;
                instanceQuad.transform.position = new Vector3(transform.position.x - _distance * i, transform.position.y,
                    transform.position.z);
                instanceQuad.transform.rotation = transform.rotation;
                _leftQuads[i] = instanceQuad;
            }
            transform.localScale = Vector3.zero;
        }
        else
        {
            this.Wait(2, () => 
            {
                float angle = 36f / (float)_samles;
                for (int i = 0; i < _samles; i++)
                {
                    GameObject instanceQuad = (GameObject)Instantiate(_quadPrefab);
                    instanceQuad.transform.SetParent(this.transform);
                    instanceQuad.name = "Quad" + i;
                    transform.rotation = Quaternion.Euler(0, 0, 1 * ((i + 1) * angle));
                    instanceQuad.transform.position = new Vector3(transform.position.x + _distance * i, transform.position.y,
                       transform.position.z);
                    instanceQuad.transform.rotation = Quaternion.Euler(transform.rotation.x,
                        transform.rotation.y, -1 * ((i + 1) * angle));

                    _rightQuads[i] = instanceQuad;                    
                }
            });
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Animate audio waves
        if (_sceneName == "MainMenu")
        {
			if (_jobs)
			{
				NativeArray<float3> LocalScaleArrayRight = new NativeArray<float3>(_samles, Allocator.TempJob);
				NativeArray<float3> LocalScaleArrayLeft = new NativeArray<float3>(_samles, Allocator.TempJob);
				NativeArray<float> SamplesArray = new NativeArray<float>(_samles, Allocator.TempJob);
				
				for (int i = 0; i < _samles; i++)
				{
					LocalScaleArrayRight[i] = _rightQuads[i].transform.localScale;
					LocalScaleArrayLeft[i] = _leftQuads[i].transform.localScale;
					SamplesArray[i] = _audioData.Samples[i];
				}
				
				VisualizerJob visualizerJob = new VisualizerJob
				{
					LocalScaleArrayRight = LocalScaleArrayRight,
					LocalScaleArrayLeft = LocalScaleArrayLeft,
					SamplesArray = SamplesArray,
					Width = _width,
					Height = _height,
					Amp = _amp,
				};
				
				JobHandle jobHandle = visualizerJob.Schedule(_samles, 8);
				jobHandle.Complete();
				
				for (int i = 0; i < _samles; i++)
				{
					_rightQuads[i].transform.localScale = LocalScaleArrayRight[i];
					_leftQuads[i].transform.localScale = LocalScaleArrayLeft[i];
					_audioData.Samples[i] = SamplesArray[i];
				}
				
				LocalScaleArrayRight.Dispose();
				LocalScaleArrayLeft.Dispose();
				SamplesArray.Dispose();				
			}
			else
			{
                for (int i = 0; i < _samles; i++)
                {  
                    _rightQuads[i].transform.localScale = new Vector3(_width, _audioData.Samples[i] * _amp  + _height, 1);
                    _leftQuads[i].transform.localScale = new Vector3(_width, _audioData.Samples[i] * _amp + _height, 1);
                }   
			}
        }
        else
        {
            if (Time.time > 2)
            {
                for (int i = 0; i < _samles; i++)
                {                                                                                                   
                    _rightQuads[i].transform.localScale = new Vector3(_width, _audioData.Samples[i] * _amp * (i / 2 + 1) + _height, 1);
                }
            }
        }
    }
}

// Jobs test
[BurstCompile]
public struct VisualizerJob : IJobParallelFor
{
	public NativeArray<float3> LocalScaleArrayRight;
	public NativeArray<float3> LocalScaleArrayLeft;
	public NativeArray<float> SamplesArray;
	public float Width, Height, Amp;
	public void Execute(int i)
	{
		LocalScaleArrayRight[i]= new float3(Width, SamplesArray[i] * Amp + Height, 1);
		LocalScaleArrayLeft[i]= new float3(Width, SamplesArray[i] * Amp + Height, 1);
	}
}
