#!/usr/bin/env swift
import Foundation
import ImageIO
import Vision

guard CommandLine.arguments.count == 2 else {
    fputs("usage: lgo_detect_body_pose.swift <image>\n", stderr)
    exit(64)
}

let input = URL(fileURLWithPath: CommandLine.arguments[1])
guard let source = CGImageSourceCreateWithURL(input as CFURL, nil),
      let image = CGImageSourceCreateImageAtIndex(source, 0, nil) else {
    fputs("cannot load image\n", stderr)
    exit(66)
}

let request = VNDetectHumanBodyPoseRequest()
do {
    try VNImageRequestHandler(cgImage: image, options: [:]).perform([request])
} catch {
    fputs("Vision request failed: \(error)\n", stderr)
    exit(70)
}
guard let observation = request.results?.first else {
    fputs("no body detected\n", stderr)
    exit(2)
}

let names: [(String, VNHumanBodyPoseObservation.JointName)] = [
    ("neck", .neck), ("root", .root),
    ("left_shoulder", .leftShoulder), ("left_elbow", .leftElbow),
    ("left_wrist", .leftWrist), ("left_hip", .leftHip),
    ("left_knee", .leftKnee), ("left_ankle", .leftAnkle),
    ("right_shoulder", .rightShoulder), ("right_elbow", .rightElbow),
    ("right_wrist", .rightWrist), ("right_hip", .rightHip),
    ("right_knee", .rightKnee), ("right_ankle", .rightAnkle),
]
let recognized = try observation.recognizedPoints(.all)
var joints: [String: [String: Any]] = [:]
for (label, name) in names {
    guard let point = recognized[name] else { continue }
    joints[label] = [
        "x": point.location.x * Double(image.width),
        "y": (1.0 - point.location.y) * Double(image.height),
        "confidence": point.confidence,
    ]
}
let output: [String: Any] = [
    "status": "VISION_BODY_POSE_DETECTED_REVIEW_ONLY",
    "input": input.path,
    "width": image.width,
    "height": image.height,
    "observationConfidence": observation.confidence,
    "coordinateOrigin": "top_left",
    "joints": joints,
    "runtimeAuthority": false,
]
let data = try JSONSerialization.data(withJSONObject: output, options: [.prettyPrinted, .sortedKeys])
FileHandle.standardOutput.write(data)
FileHandle.standardOutput.write(Data([0x0a]))
