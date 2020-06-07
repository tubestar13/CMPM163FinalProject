using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]

public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    public float[] samples = new float[512];
    public float[] freqBand = new float[8];
    public float[] bandBuffer = new float[8];
    public float[] _bufferDecrease = new float[8];

    public float[] freqBandHighest = new float[8];
    public float[] audioBand = new float[8];
    public float[] audioBandBuffer = new float[8];
    public float _audioProfile;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void AudioProfile(float audioProfile)
    {
        for(int i=0; i < 8; i++)
        {
            freqBandHighest[i] = audioProfile;
        }
    }
    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand[i] > freqBandHighest[i])
            {
                freqBandHighest[i] = freqBand[i];
            }
            audioBand[i] = (freqBand[i] / freqBandHighest[i]);
            audioBandBuffer[i] = (bandBuffer[i] / freqBandHighest[i]);
        }
    }
    void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBand[i];
                _bufferDecrease[i] = 0.005f;
            }
            if (freqBand[i] < bandBuffer[i])
            {
                bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f; //accelerate the descreasing
            }
        }
    }
    void MakeFrequencyBands()
    {
        /* The frequency ranges 
         * 20-60 Hz Sub Bass
         * 60-250 Hz Bass
         * 250-500 Hz Low Midrange
         * 500-2000 Hz Midrange
         * 2000-4000 Hz Upper Midrange
         * 4000-6000 Hz Presence
         * 6000-20000 Hz Brilliance
         * 
         * We have 512 samples from 0Hz ~ 20000Hz
         * this equates to roughly ~40 Hz per sample
         * 0 - 2 samples = 80 Hz | 0 - 80 Hz
         * 1 - 4 samples = 160 Hz | 80 Hz - 240 Hz
         * 2 - 8 samples = 320 Hz | 240 Hz - 560 Hz
         * 3 - 16 samples = 640 Hz | 560 Hz - 1200 Hz
         * 4 - 32 samples = 1280 Hz | 1200 Hz - 2480 Hz
         * 5 - 64 samples = 2560 Hz | 2480 Hz - 5040 Hz
         * 6 - 128 samples = 5120 Hz | 5040 Hz - 10160 Hz
         * 7 - 256 samples + 2 samples = 10320 Hz | 10160 Hz - 20480 Hz
         * the +2 samples is because we miss 2 samples from just the increments
         */

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count+1); //we do like this because the higher range freq are really smol
                count++;
            }

            average /= count;

            freqBand[i] = average * 10;
        }
    }
}
