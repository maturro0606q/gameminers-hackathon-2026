using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace GameMiners.Gameplay
{
    internal sealed class GameAudio : IDisposable
    {
        private const string VolumeKey = "GameMiners.MusicVolume";
        private const int SampleRate = 22050;
        private const float MusicGain = 0.56f;
        private readonly AudioSource soundtrack;
        private readonly AudioSource effects;
        private readonly AudioSource dialogueSource;
        private readonly Dictionary<string, AudioClip> voices = new();
        private AudioClip ownedTrack;
        private AudioClip throneTrack;
        private bool throneRequested;
        private readonly Dictionary<Cue, AudioClip> cues = new();
        private float fade = 1f;

        public enum Cue { Serve, Door, Capture, Swing, Hit, Hurt, Clear, Bomb, Guard, Crush, Pop, Loot, Dash, Chest }

        public GameAudio(MonoBehaviour owner, int chapter, AudioClip assigned)
        {
            soundtrack = owner.gameObject.AddComponent<AudioSource>();
            soundtrack.loop = true;
            effects = owner.gameObject.AddComponent<AudioSource>();
            effects.volume = 0.63f;
            dialogueSource = owner.gameObject.AddComponent<AudioSource>();
            dialogueSource.volume = 0.87f;
            ApplyVolume();
            string track = chapter < 2 ? "IceCreamTruck" : chapter == 2 ? "MagicPuzzle" : "EpicBossBattle";
            owner.StartCoroutine(LoadMusic(track, clip =>
            {
                ownedTrack = clip;
                if (!throneRequested) PlayMusic(clip != null ? clip : assigned ?? (ownedTrack = MakeTrack(chapter)));
            }));
            if (chapter == 3)
                owner.StartCoroutine(LoadMusic("SpookyWaltz", clip =>
                {
                    throneTrack = clip;
                    if (throneRequested && clip != null) PlayMusic(clip);
                }));
        }

        private static IEnumerator LoadMusic(string track, Action<AudioClip> ready)
        {
            string path = PortableModel.PathFor(track + ".ogg");
            if (File.Exists(path))
            {
                using var request = UnityWebRequestMultimedia.GetAudioClip(new Uri(path).AbsoluteUri, AudioType.OGGVORBIS);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var clip = DownloadHandlerAudioClip.GetContent(request);
                    clip.name = track;
                    ready(clip);
                    yield break;
                }
            }
            ready(null);
        }

        private void PlayMusic(AudioClip clip)
        {
            soundtrack.clip = clip;
            soundtrack.Play();
            Debug.Log("GAMEMINERS_MUSIC " + clip.name);
        }

        public void PlayThroneMusic()
        {
            throneRequested = true;
            soundtrack.Stop();
            if (throneTrack != null) PlayMusic(throneTrack);
        }

        private static float? sessionVolume;
        public float Volume => sessionVolume ?? PlayerPrefs.GetFloat(VolumeKey, 0.65f);

        public void SetVolume(float value)
        {
            sessionVolume = Mathf.Clamp01(value);
            try
            {
                PlayerPrefs.SetFloat(VolumeKey, sessionVolume.Value);
                PlayerPrefs.Save();
            }
            catch (PlayerPrefsException)
            {
                // Settings still apply for the running session when preferences are unavailable.
            }
            ApplyVolume();
        }

        public void Fade(float multiplier)
        {
            fade = Mathf.Clamp01(multiplier);
            ApplyVolume();
        }

        public void Play(Cue cue, float pitch = 1f)
        {
            if (!cues.TryGetValue(cue, out var clip))
            {
                clip = MakeCue(cue);
                cues.Add(cue, clip);
            }
            effects.pitch = Mathf.Clamp(pitch, 0.7f, 1.65f);
            effects.PlayOneShot(clip);
        }

        public float Speak(string line)
        {
            if (!voices.TryGetValue(line, out var clip))
            {
                string file = PortableModel.PathFor(Path.Combine("Voices", line + ".wav"));
                if (!File.Exists(file)) return 0f;
                clip = ReadVoice(line, file);
                if (clip == null) return 0f;
                voices.Add(line, clip);
            }
            dialogueSource.Stop();
            dialogueSource.clip = clip;
            dialogueSource.Play();
            return clip.length;
        }

        private static AudioClip ReadVoice(string name, string path)
        {
            using var input = new BinaryReader(File.OpenRead(path));
            if (input.ReadUInt32() != 0x46464952) return null;
            input.ReadUInt32();
            if (input.ReadUInt32() != 0x45564157) return null;
            int channels = 0, frequency = 0, bits = 0;
            while (input.BaseStream.Position + 8 < input.BaseStream.Length)
            {
                uint chunk = input.ReadUInt32();
                uint bytes = input.ReadUInt32();
                long end = input.BaseStream.Position + bytes + (bytes & 1);
                if (end > input.BaseStream.Length + 1) return null;
                if (chunk == 0x20746d66)
                {
                    if (bytes < 16 || input.ReadUInt16() != 1) return null;
                    channels = input.ReadUInt16();
                    frequency = input.ReadInt32();
                    input.ReadInt32();
                    input.ReadUInt16();
                    bits = input.ReadUInt16();
                }
                else if (chunk == 0x61746164 && bits == 16 && channels > 0)
                {
                    var samples = new float[(int)(bytes / 2)];
                    for (int n = 0; n < samples.Length; n++) samples[n] = input.ReadInt16() / 32768f;
                    var clip = AudioClip.Create(name, samples.Length / channels, channels, frequency, false);
                    clip.SetData(samples, 0);
                    return clip;
                }
                input.BaseStream.Position = end;
            }
            return null;
        }

        private void ApplyVolume() => soundtrack.volume = Volume * fade * MusicGain;

        private static AudioClip MakeTrack(int chapter)
        {
            bool combat = chapter >= 2;
            bool boss = chapter == 3;
            float beat = 60f / (boss ? 118f : combat ? 109f : 89f);
            var samples = new float[Mathf.RoundToInt(64f * beat * SampleRate)];
            int[] harmony = boss ? new[] { 45, 41, 46, 40 } : combat ? new[] { 48, 44, 51, 46 } : new[] { 48, 45, 50, 43 };
            int[] motif = boss ? new[] { 12, 7, 15, 13, 7, 10, 6, 3 }
                : combat ? new[] { 12, 15, 7, 10, 12, 7, 14, 15 }
                : new[] { 12, 19, 24, 16, 19, 26, 23, 19 };

            for (int bar = 0; bar < 16; bar++)
            {
                int root = harmony[bar % harmony.Length];
                float start = bar * 4f * beat;
                int third = combat || bar % 4 == 1 ? 3 : 4;
                Tone(samples, start, beat * 3.8f, root, 0.082f, 0);
                Tone(samples, start, beat * 3.8f, root + third + 12, 0.023f, 0);
                Tone(samples, start, beat * 3.8f, root + 19, 0.022f, 0);
                for (int step = 0; step < 8; step++)
                {
                    if (!combat && step % 4 == 3) continue;
                    int index = (step + (bar / 4 % 2) * 2) % motif.Length;
                    Tone(samples, start + step * beat / 2f, beat * 0.44f, root + motif[index], 0.092f, 1);
                }
                if (!combat) continue;
                Tone(samples, start, beat * 0.33f, 35, 0.19f, 2);
                Tone(samples, start + 2f * beat, beat * 0.33f, 35, 0.15f, 2);
                Tone(samples, start + 3.5f * beat, beat * 0.15f, 61, 0.057f, 2);
            }

            var clip = AudioClip.Create("Tema original " + chapter, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static void Tone(float[] samples, float start, float length, int midi, float gain, int voice)
        {
            int offset = Mathf.RoundToInt(start * SampleRate);
            int count = Mathf.Min(Mathf.RoundToInt(length * SampleRate), samples.Length - offset);
            double frequency = 440.0 * Math.Pow(2, (midi - 69) / 12.0);
            double phase = 2 * Math.PI * frequency / SampleRate;
            for (int i = 0; i < count; i++)
            {
                double t = (double)i / count;
                double envelope = Math.Min(1, t * 35) * Math.Min(1, (1 - t) * 9);
                double signal = voice switch
                {
                    0 => Math.Sin(phase * i) + 0.26 * Math.Sin(phase * i * 2.003),
                    1 => (Math.Sin(phase * i) + 0.32 * Math.Sin(phase * i * 2.75)) * Math.Exp(-2.9 * t),
                    _ => Math.Sin(phase * i * (1.55 - t * 0.8)) * Math.Exp(-4.5 * t)
                };
                samples[offset + i] += (float)(gain * envelope * signal);
            }
        }

        private static AudioClip MakeCue(Cue cue)
        {
            (float seconds, float low, float high, float grit) = cue switch
            {
                Cue.Serve => (0.22f, 507f, 760f, 0.02f),
                Cue.Door => (0.26f, 122f, 79f, 0.29f),
                Cue.Capture => (1.30f, 267f, 51f, 0.38f),
                Cue.Swing => (0.15f, 146f, 68f, 0.49f),
                Cue.Hit => (0.19f, 158f, 86f, 0.34f),
                Cue.Hurt => (0.24f, 195f, 90f, 0.16f),
                Cue.Clear => (0.46f, 309f, 620f, 0.02f),
                Cue.Crush => (0.24f, 133f, 44f, 0.42f),
                Cue.Pop => (0.28f, 276f, 62f, 0.31f),
                Cue.Guard => (0.16f, 660f, 383f, 0.24f),
                Cue.Loot => (0.13f, 775f, 1175f, 0.02f),
                Cue.Dash => (0.17f, 161f, 97f, 0.74f),
                Cue.Chest => (0.36f, 243f, 540f, 0.24f),
                _ => (0.53f, 82f, 34f, 0.58f)
            };
            var samples = new float[Mathf.RoundToInt(seconds * SampleRate)];
            var noise = new System.Random(53 + (int)cue);
            double phase = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = (float)i / samples.Length;
                phase += Math.PI * 2 * Mathf.Lerp(low, high, t) / SampleRate;
                float envelope = Mathf.Clamp01(t * 35f) * Mathf.Pow(1f - t, 2f);
                samples[i] = envelope * (float)((1f - grit) * Math.Sin(phase) + grit * (noise.NextDouble() * 2 - 1));
            }
            var clip = AudioClip.Create(cue.ToString(), samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public void Dispose()
        {
            if (ownedTrack != null) UnityEngine.Object.Destroy(ownedTrack);
            if (throneTrack != null) UnityEngine.Object.Destroy(throneTrack);
            foreach (var clip in cues.Values) UnityEngine.Object.Destroy(clip);
            foreach (var voice in voices.Values) UnityEngine.Object.Destroy(voice);
        }
    }
}
