using System;
using System.IO;

namespace Wavplay.WaveGenerator
{
    public class WaveGenerator
    {
        // Header, Format, Data chunks
        WaveHeader header;
        WaveFormatChunk format;
        WaveDataChunk data;

        public WaveGenerator()
        {
            header = new WaveHeader();
            format = new WaveFormatChunk();
            data = new WaveDataChunk();

            GeneratWaveArray(0);
            data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
        }

        public WaveGenerator(short[] shortArray)
        {
            header = new WaveHeader();
            format = new WaveFormatChunk();
            data = new WaveDataChunk();

            data.shortArray = shortArray;
            data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
        }
        public WaveGenerator(double freq, WaveExampleType type = WaveExampleType.ExampleSineWave, int volum = 100, double duration = 1)
        {
            // Init chunks
            header = new WaveHeader();
            format = new WaveFormatChunk();
            data = new WaveDataChunk();

            GeneratWaveArray(freq, type, volum, duration);
            data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
        }

        public short[] GeneratWaveArray(double freq, WaveExampleType type = WaveExampleType.Default, int volum = 100, double duration = 1)
        {
            if (duration == 0) duration = 0.01;
            uint numSamples = (uint)(duration * format.dwSamplesPerSec * format.wChannels);
            // Initialize the 16-bit array
            data.shortArray = new short[numSamples];
            if (volum > 100) volum = 100;
            int amplitude = 32765 * volum / 100;  // Max amplitude for 16-bit audio
                                                  // freq = 329.0f;   // Concert A: 440Hz
                                                  // The "angle" used in the function, adjusted for the number of channels and sample rate.
                                                  // This value is like the period of the wave.
            double t = (Math.PI * 4 * freq) / (format.dwSamplesPerSec * format.wChannels);

            // Fill the data array with sample data
            switch (type)
            {
                //Sine
                case WaveExampleType.ExampleSineWave:

                    for (uint i = 0; i < numSamples / 2 - 1; i++)
                    {
                        // Fill with a simple sine wave at max amplitude
                        for (int channel = 0; channel < format.wChannels; channel++)
                        {
                            if (channel == 0) data.shortArray[2 * i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                            //else data.shortArray[2*i + channel] = 0;
                            else data.shortArray[2 * i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i + Math.PI));
                        }
                    }
                    break;

                //Square
                case WaveExampleType.ExampleSquareWave:

                    for (int i = 0; i < numSamples - 1; i++)
                    {
                        for (int channel = 0; channel < format.wChannels; channel++)
                            data.shortArray[i + channel] = data.shortArray[i] = Convert.ToInt16(amplitude * Math.Sign(Math.Sin(t * i)));
                    }
                    break;

                //Sawtooth
                case WaveExampleType.ExampleSawtoothWave:
                    // Determine the number of samples per wavelength
                    int samplesPerWavelength = Convert.ToInt32(format.dwSamplesPerSec / (freq / format.wChannels));
                    // Determine the amplitude step for consecutive samples
                    short ampStep = Convert.ToInt16((amplitude * 2) / samplesPerWavelength);
                    // Temporary sample value, added to as we go through the loop
                    short tempSample = (short)-amplitude;
                    // Total number of samples written so we know when to stop
                    int totalSamplesWritten = 0;
                    while (totalSamplesWritten < numSamples)
                    {
                        tempSample = (short)-amplitude;

                        for (uint i = 0; i < samplesPerWavelength && totalSamplesWritten < numSamples; i++)
                        {
                            for (int channel = 0; channel < format.wChannels; channel++)
                            {
                                tempSample += ampStep;
                                data.shortArray[totalSamplesWritten] = tempSample;

                                totalSamplesWritten++;
                            }
                        }
                    }
                    break;

                //Triangle
                case WaveExampleType.ExampleTriangleWave:
                    int samplesPerWavelengthTr = Convert.ToInt32(format.dwSamplesPerSec / (freq / format.wChannels));
                    // Determine the amplitude step for consecutive samples
                    short ampStepTr = Convert.ToInt16((amplitude * 2) / samplesPerWavelengthTr);
                    // Temporary sample value, added to as we go through the loop
                    short tempSampleTr = (short)-amplitude;
                    // Total number of samples written so we know when to stop

                    for (int i = 0; i < numSamples - 1; i++)
                    {
                        for (int channel = 0; channel < format.wChannels; channel++)
                        {
                            // Negate ampstep whenever it hits the amplitude boundary
                            if (Math.Abs((int)tempSampleTr) > amplitude) ampStepTr = (short)-ampStepTr;
                            tempSampleTr += ampStepTr;
                            data.shortArray[i + channel] = tempSampleTr;
                        }
                    }
                    break;

                // Random
                case WaveExampleType.ExampleRandomWave:
                    Random rnd = new Random();
                    short randomValue = 0;
                    for (int i = 0; i < numSamples; i++)
                    {
                        randomValue = Convert.ToInt16(rnd.Next(-amplitude, amplitude));
                        data.shortArray[i] = randomValue;
                    }
                    break;

                default:
                    //data.shortArray = shortArray;
                    break;
            }
            return data.shortArray;
        }

        public void Save(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);// Create a file (it always overwrites)
            BinaryWriter writer = new BinaryWriter(fileStream);// Use BinaryWriter to write the bytes to the file
            // Write the header
            writer.Write(header.sGroupID.ToCharArray());
            writer.Write(header.dwFileLength);
            writer.Write(header.sRiffType.ToCharArray());

            // Write the format chunk
            writer.Write(format.sChunkID.ToCharArray());
            writer.Write(format.dwChunkSize);
            writer.Write(format.wFormatTag);
            writer.Write(format.wChannels);
            writer.Write(format.dwSamplesPerSec);
            writer.Write(format.dwAvgBytesPerSec);
            writer.Write(format.wBlockAlign);
            writer.Write(format.wBitsPerSample);

            // Write the data chunk
            writer.Write(data.sChunkID.ToCharArray());
            writer.Write(data.dwChunkSize);
            foreach (short dataPoint in data.shortArray)
                writer.Write(dataPoint);

            writer.Seek(4, SeekOrigin.Begin);
            uint filesize = (uint)writer.BaseStream.Length;
            writer.Write(filesize - 8);

            // Clean up
            writer.Close();
            fileStream.Close();
        }

        public WaveDataChunk GetData()
        {
            return data;
        }

        public WaveFormatChunk GetFormat()
        {
            return format;
        }
    }
}
