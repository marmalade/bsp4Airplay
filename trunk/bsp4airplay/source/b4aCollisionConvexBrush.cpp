#include <IwTextParserITX.h>
#include "b4aCollisionConvexBrush.h"
#include "Bsp4Airplay.h"

using namespace Bsp4Airplay;

IW_CLASS_FACTORY(Cb4aCollisionConvexBrush);
IW_MANAGED_IMPLEMENT(Cb4aCollisionConvexBrush)

//Constructor
Cb4aCollisionConvexBrush::Cb4aCollisionConvexBrush()
{
}

//Desctructor
Cb4aCollisionConvexBrush::~Cb4aCollisionConvexBrush()
{
}
void  Cb4aCollisionConvexBrush::Serialise ()
{
	planes.SerialiseHeader();
	for (uint32 i=0; i<planes.size(); ++i)
	{
		planes[i].plane.v.Serialise();
		IwSerialiseInt32(planes[i].plane.k);
		if (IwSerialiseIsReading())
			planes[i].calc = GetDistanceCalculator(planes[i].plane);
		
	}
}

b4aCollisionResult Cb4aCollisionConvexBrush::TraceSphere(int32 sphere, Cb4aTraceContext& context) const
{
	b4aCollisionResult res  = COLLISION_NONE;
	for (uint32 planeIndex=0;planeIndex<planes.size(); ++planeIndex)
	{
		const Cb4aCollisionMeshSoupPlane& plane = planes[planeIndex];
		CIwVec3 shift = plane.plane.v*(-sphere);

		iwfixed fromDist = plane.calc(context.from+shift,plane.plane);
		if (fromDist < -b4aCollisionEpsilon)
			continue;
		iwfixed toDist = plane.calc(context.to+shift,plane.plane);
		if (toDist > b4aCollisionEpsilon)
			continue;
		if (fromDist <= toDist)
			continue;
		CIwVec3 point;
		if (fromDist <= 0)
		{
			point = context.from+shift;
			fromDist = 0;
		}
		else if (toDist >= 0)
			point = context.to+shift;
		else
			point = b4aLerp(context.from,context.to,fromDist,toDist)+shift;

		for (uint32 otherIndex=0;otherIndex<planes.size(); ++otherIndex)
		{
			if (otherIndex == planeIndex)
				continue;
			const Cb4aCollisionMeshSoupPlane& other = planes[otherIndex];
			if (other.calc(point,other.plane)>b4aCollisionEpsilon+sphere)
			{
				goto noCollision;
			}
		}
		context.to = point-shift;
		context.collisionNormal = plane.plane.v;
		context.collisionPlaneD = plane.plane.k;
		if (fromDist <= 0)
			return COLLISION_ATSTART;
		res = COLLISION_SOMEWHERE;
noCollision: ;
		
	}
	return res;
}
b4aCollisionResult Cb4aCollisionConvexBrush::TraceLine(Cb4aTraceContext& context) const 
{
	b4aCollisionResult res  = COLLISION_NONE;
	for (uint32 planeIndex=0;planeIndex<planes.size(); ++planeIndex)
	{
		const Cb4aCollisionMeshSoupPlane& plane = planes[planeIndex];
		iwfixed fromDist = plane.calc(context.from,plane.plane);
		if (fromDist < -b4aCollisionEpsilon)
			continue;
		iwfixed toDist = plane.calc(context.to,plane.plane);
		if (toDist > b4aCollisionEpsilon)
			continue;
		if (fromDist <= toDist)
			continue;
		CIwVec3 point;
		if (fromDist <= 0)
		{
			point = context.from;
			fromDist = 0;
		}
		else if (toDist >= 0)
			point = context.to;
		else
			point = b4aLerp(context.from,context.to,fromDist,toDist);

		for (uint32 otherIndex=0;otherIndex<planes.size(); ++otherIndex)
		{
			if (otherIndex == planeIndex)
				continue;
			const Cb4aCollisionMeshSoupPlane& other = planes[otherIndex];
			if (other.calc(point,other.plane)>b4aCollisionEpsilon)
			{
				goto noCollision;
			}
		}
		context.to = point;
		context.collisionNormal = plane.plane.v;
		context.collisionPlaneD = plane.plane.k;
		if (fromDist <= 0)
			return COLLISION_ATSTART;
		res = COLLISION_SOMEWHERE;
noCollision: ;
		
	}
	return res;
}
#ifdef IW_BUILD_RESOURCES

// function invoked by the text parser when parsing attributes for objects of this type
bool Cb4aCollisionConvexBrush::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_planes", pAttrName))
	{
		int num_planes;
		pParser->ReadInt32(&num_planes);
		planes.set_capacity(num_planes);
		return true;
	}
	if (!strcmp("plane", pAttrName))
	{
		iwfixed planeValues[4];
		pParser->ReadInt32Array(&planeValues[0],4);
		planes.push_back();
		planes.back().plane = Cb4aPlane(CIwVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser, pAttrName);
}


// function invoked by the text parser when the object definition end is encountered
void Cb4aCollisionConvexBrush::ParseClose(CIwTextParserITX* pParser)
{
	dynamic_cast<Ib4aColliderContainer*>(pParser->GetObject(-1))->AddCollider(this);
}
#endif