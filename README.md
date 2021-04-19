# Vision - CullingGroup API for everyone

**Created by Hiroya Aramaki ([Makihiro](https://twitter.com/makihiro_dev))**

[![Tests](https://github.com/mackysoft/Vision/actions/workflows/tests.yaml/badge.svg)](https://github.com/mackysoft/Vision/actions/workflows/tests.yaml)
[![Build](https://github.com/mackysoft/Vision/actions/workflows/build.yaml/badge.svg)](https://github.com/mackysoft/Vision/actions/workflows/build.yaml)

## What is CullingGroup API ?

![TreasureRogue_Culling](https://user-images.githubusercontent.com/13536348/115215242-47d33380-a13e-11eb-816c-c10df930f1aa.gif)

CullingGroup offers a way to integrate your own systems into Unity’s culling and LOD pipeline.

- Simulating a crowd of people, while only having full GameObjects for the characters that are actually visible right now.
- Building a GPU particle system driven by Graphics.DrawProcedural, but skipping rendering particle systems that are behind a wall.
- Tracking which spawn points are hidden from the camera in order to spawn enemies without the player seeing them ‘pop’ into view.
- Switching characters from full-quality animation and AI calculations when close, to lower-quality cheaper behaviour at a distance.
- Having 10,000 marker points in your scene and efficiently finding out when the player gets within 1m of any of them.

> Unity Manual: https://docs.unity3d.com/Manual/CullingGroupAPI.html


## Why Vision ?

CullingGroup is a great feature, but its implementation is a difficult because it can only be accessed through scripts and its usage is quirky.

Vision has made such a CullingGroup available to everyone.

#### Vision Features

- Components for easy access to CullingGroup.
- Intuitive visual editor.
- High performance


## <a id="index" href="#index"> Table of Contents </a>

- [📥 Installation](#installation)
- [🔰 Usage](#usage)
  - [1. Create the CullingGroupProxy](#create-the-culling-group-proxy)
  - [2. Attach the CullingTargetBehaviour](#attach-the-culling-target-behaviour)
  - [3. Receive a callback](#receive-callback)
- [⚒Utilities](#utilities)
  - [Culling Target Renderers](#culling-target-renderers)
- [📔 Author Info](#author-info)
- [📜 License](#license)


# <a id="installation" href="#installation"> 📥 Installation </a>

Download any version from releases.

Releases: https://github.com/mackysoft/Vision/releases

#### Install via Open UPM

Or, you can install this package from the [Open UPM](https://openupm.com/packages/com.mackysoft.vision/) registry.

More details [here](https://openupm.com/).

```
openupm add com.mackysoft.vision
```

# <a id="usage" href="#requirements"> 🔰 Usage </a>

## <a id="create-the-culling-group-proxy" href="#create-the-culling-group-proxy"> 1. Create the CullingGroupProxy </a>

First, create a `CullingGroupProxy`, which is the core of Vision.

#### 1. From the `Tools/Vision/Create New CullingGroupProxy` menu, create a `CullingGroupProxy`.

![NewCullingGroupProxy](https://user-images.githubusercontent.com/13536348/111070665-0c798f80-8516-11eb-8ed6-d1f87cc31b61.jpg)


#### 2. Set the key to the created `CullingGroupProxy`.

Set the key by clicking the `Key` popup in `CullingGroupProxy`. By default, there is a `Main` key.

> If you don't see the key, open settings from the `<add key>` or `Tools/Vision/Open Settings` menu.
You can define a new key in the `GroupKeyDefinitions` list.
> 
> ![VisionSettings](https://user-images.githubusercontent.com/13536348/111070868-033cf280-8517-11eb-852c-386f00ad2f45.jpg)


#### 3. Distances

By setting `DistanceReferencePoint` and `BoundingDistances` of `CullingGroupProxy`, you can calculate the relative distance between a reference point (for example, camera, player) and the bounding sphere's.

![CullingGroupPRoxy_Distances](https://user-images.githubusercontent.com/13536348/111077905-465a8e00-8536-11eb-92c1-657f73c1e7df.jpg)

If `BoundingDistances` is set to { 10, 20, 30 },

|Distance Level|Range|
|:--|:--|
|Level 0|0m ~ 10m|
|Level 1|10m ~ 20m|
|Level 2|20m ~ 30m|

The last level and beyond will be treated as invisible.

> `BoundingDistances` can be adjusted visually from the scene view !

![CullingTargetProxy_Distances_Gizmo](https://user-images.githubusercontent.com/13536348/111077910-4c506f00-8536-11eb-917b-dd35a4bdc6bd.jpg)

## <a id="attach-the-culling-target-behaviour" href="#attach-the-culling-target-behaviour"> 2. Attach the CullingTargetBehaviour </a>

`CullingTargetBehaviour` is a component that attaches to the object to be culled.


#### 1. Attach `CullingTargetBehaviour` from `Component/MackySoft/Vision/Culling Group Behaviour` menu.

Attach this component to objects that you want to show/hide depending on their visibility, or change their quality depending on their relative distance.

![CullingTargetBehaviour](https://user-images.githubusercontent.com/13536348/111072032-333ac480-851c-11eb-8821-0b4f766e1e34.jpg)


#### 2. Set the group key to the attached `CullingGroupBehaviour`.

The `GroupKey` of the `CullingGroupBehaviour` is used to find a `CullingGroupProxy` with the same key specified,
and the `CullingGroupBehaviour` will be registered with the found `CullingGroupProxy`.


#### 3. Adjust the radius of `CullingGroupBehaviour`.

The bounding sphere of the `CullingGroupBehaviour` is used to calculate the visibility by the `CullingGroup`. Completely enclose the object you want to cull with the bounding sphere.

> Radius can be adjusted visually from the scene view !

![CullingTargetBehaviour_Idle](https://user-images.githubusercontent.com/13536348/111072309-57e36c00-851d-11eb-8196-8c38b2af62c1.jpg)

Also, during play mode, the color changes depending on visibility.

<img src="https://user-images.githubusercontent.com/13536348/111074733-bf061e00-8527-11eb-8e19-e6796e56c63a.jpg" height="200" title="CullingTargetBehaviour_Visible" />
<img src="https://user-images.githubusercontent.com/13536348/111074737-c6c5c280-8527-11eb-9275-220103d8d59a.jpg" height="200" title="CullingTargetBehaviour_Invisible" />

#### 4. Select the `BoundingSphereUpdateMode`.

This is an important value for performance.

`BoundingSphereUpdateMode` is set to `Dynamic` by default. This means that the transform of bounding sphere will be updated every frame.

However, there are some objects that do not move. In such cases, you can avoid the extra update cost by setting the `BoundingSphereUpdateMode` to `Static`.

## <a id="receive-callback" href="#receive-callback"> 3. Receive a callback </a>

Use `CullinTargetBehaviour.OnStateChanged` callback to respond to changes in the visibility and distance state of the bounding sphere.

```cs
using UnityEngine;
using MackySoft.Vision;

[RequireComponent(typeof(CullingTargetBehaviour))]
public class ReceiveCallbackExample : MonoBehaviour {

    void Awake () {
        var cullingTarget = GetComponent<ICullingTarget>();
        cullingTarget.OnStateChanged += OnStateChanged;
    }

    void OnStaeteChanged (CullingGroupEvent ev) {
        if (ev.isVisible) {
            Debug.Log("Visible!");
        } else {
            Debug.Log("Invisible!");
        }
    }

}
```

## <a id="utilities" href="#utilities"> Utilities </a>

Vision provides utility components that can be used with no coding.


### <a id="culling-target-renderers" href="#culling-target-renderers"> Culling Target Renderers </a>

Enable / Disable the specified renderer's depending on the visibility of the bounding sphere.

![CullingTargetRenderers](https://user-images.githubusercontent.com/13536348/111073973-5cf7e980-8524-11eb-9b84-ab95c263940c.jpg)


# <a id="author-info" href="#author-info"> 📔 Author Info </a>

Hiroya Aramaki is a indie game developer in Japan.

- Blog: [https://mackysoft.net/blog](https://mackysoft.net/blog)
- Twitter: [https://twitter.com/makihiro_dev](https://twitter.com/makihiro_dev)


# <a id="license" href="#license"> 📜 License </a>

This library is under the MIT License.
