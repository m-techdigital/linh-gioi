using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinhGioi.ArchitectureProbe
{
    [Serializable]
    public sealed class LgoSkeletal2DBindProfile
    {
        [Serializable]
        public sealed class Pose
        {
            public string pose;
            public Landmark[] landmarks;
        }

        [Serializable]
        public sealed class Landmark
        {
            public string id;
            public float[] xy;
        }

        public readonly struct Segment
        {
            public Segment(string id, int parent, Vector2 start, Vector2 end)
            {
                Id = id; Parent = parent; Start = start; End = end;
            }
            public string Id { get; }
            public int Parent { get; }
            public Vector2 Start { get; }
            public Vector2 End { get; }
        }

        public string status;
        public string guideId;
        public string sourceSpaceProfile;
        public int[] sourceCanvas;
        public string coordinateOrigin;
        public int originX;
        public int groundY;
        public Pose[] poses;

        public bool RuntimeAuthority => string.Equals(status, "ACCEPTED_BIND_AUTHORITY", StringComparison.Ordinal);

        public static LgoSkeletal2DBindProfile Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Bind profile JSON is required", nameof(json));
            var profile = JsonUtility.FromJson<LgoSkeletal2DBindProfile>(json);
            if (profile == null) throw new ArgumentException("Bind profile JSON is invalid", nameof(json));
            profile.ValidateHeader();
            return profile;
        }

        public Segment[] CreateReviewSegments()
        {
            ValidateHeader();
            var idle = poses?.SingleOrDefault(item => item != null && item.pose == "idle")
                ?? throw new InvalidOperationException("Bind profile must contain exactly one idle pose");
            var points = idle.landmarks?.ToDictionary(item => item.id, Point)
                ?? throw new InvalidOperationException("Idle pose landmarks are missing");
            Vector2 Require(string id) => points.TryGetValue(id, out var point)
                ? point : throw new InvalidOperationException("Bind profile is missing landmark " + id);

            var nearHip = Require("near_hip");
            var farHip = Require("far_hip");
            var pelvis = (nearHip + farHip) * .5f;
            var nearAnkle = Require("near_ankle");
            var farAnkle = Require("far_ankle");
            var segments = new[]
            {
                new Segment("pelvis", -1, pelvis, Require("neck")),
                new Segment("head", 0, Require("neck"), Require("crown")),
                new Segment("near_upper_arm", 0, Require("near_shoulder"), Require("near_elbow")),
                new Segment("near_forearm", 2, Require("near_elbow"), Require("near_wrist")),
                new Segment("far_upper_arm", 0, Require("far_shoulder"), Require("far_elbow")),
                new Segment("far_forearm", 4, Require("far_elbow"), Require("far_wrist")),
                new Segment("near_thigh", 0, nearHip, Require("near_knee")),
                new Segment("near_shin", 6, Require("near_knee"), new Vector2(nearAnkle.x, groundY)),
                new Segment("far_thigh", 0, farHip, Require("far_knee")),
                new Segment("far_shin", 8, Require("far_knee"), new Vector2(farAnkle.x, groundY)),
            };
            if (segments.Any(segment => Vector2.Distance(segment.Start, segment.End) < 1f))
                throw new InvalidOperationException("Bind profile contains a collapsed segment");
            return segments;
        }

        private static Vector2 Point(Landmark landmark)
        {
            if (landmark == null || string.IsNullOrWhiteSpace(landmark.id) || landmark.xy == null || landmark.xy.Length != 2
                || float.IsNaN(landmark.xy[0]) || float.IsNaN(landmark.xy[1]))
                throw new InvalidOperationException("Bind landmark must contain id and finite xy");
            return new Vector2(landmark.xy[0], landmark.xy[1]);
        }

        private void ValidateHeader()
        {
            if (string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(guideId)
                || sourceSpaceProfile != "lgo_character_canvas_1024x1536_v1"
                || sourceCanvas == null || sourceCanvas.Length != 2 || sourceCanvas[0] != 1024 || sourceCanvas[1] != 1536
                || coordinateOrigin != "top_left" || originX != 512 || groundY <= 0 || groundY > 1536)
                throw new InvalidOperationException("Bind profile header does not match the registered LGO source-space contract");
        }
    }
}
