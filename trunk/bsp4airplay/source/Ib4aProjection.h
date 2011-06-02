#pragma once
#include <IwManaged.h>
#include <IwGx.h>
#include "b4aPlane.h"

namespace Bsp4Airplay
{
	class Cb4aLevelVertexBuffer;
	class Cb4aLevelVBSubcluster;
	class Cb4aLevel;

	class Ib4aProjection
	{
		public:

		//Constructor
		inline Ib4aProjection(){};
		//Desctructor
		inline virtual ~Ib4aProjection(){};

		virtual void Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)=0;
		virtual void Flush()=0;
	};

	struct Cb4aFlashlightProjectionVertex
	{
		CIwSVec3 pos;
		CIwSVec2 uv0;

		static void Lerp(Cb4aFlashlightProjectionVertex*dst, const Cb4aFlashlightProjectionVertex& v0, int32 d0, const Cb4aFlashlightProjectionVertex& v1, int32 d1);
	};
	struct Cb4aFlashlightProjectionFace
	{
		Cb4aFlashlightProjectionVertex vertices[3];
	};

	class Cb4aFlashlightProjection:public Ib4aProjection
	{
		CIwTexture* texure;
		CIwMat matrix;
		CIwVec3 whz;
		int32 near;
		CIwBBox projectionBox;
		CIwArray<CIwSVec3> positions;
		CIwArray<CIwSVec2> uv0;
		CIwArray<CIwColour> col;
		CIwArray<uint16> indices;
		Cb4aPlane frustum[6];
	public:
		Cb4aFlashlightProjection();
		void Prepare(CIwTexture* tex, const CIwMat& mat, const CIwVec3 & _whz);
		void Clear();
		virtual void Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry);
		virtual void Flush();
	protected:
		void Add(Cb4aFlashlightProjectionFace& face,int plane);
	};
	class Cb4aSkyProjection:public Ib4aProjection
	{
	public:
		Cb4aSkyProjection();
		virtual void Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry);
		virtual void Flush();
	};
}