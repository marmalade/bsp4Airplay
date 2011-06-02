#include <IwTextParserITX.h>
#include "b4aCollisionMeshSoup.h"
#include "Bsp4Airplay.h"

using namespace Bsp4Airplay;

IW_CLASS_FACTORY(Cb4aCollisionMeshSoup);
IW_MANAGED_IMPLEMENT(Cb4aCollisionMeshSoup)

//Constructor
Cb4aCollisionMeshSoup::Cb4aCollisionMeshSoup()
{
}

//Desctructor
Cb4aCollisionMeshSoup::~Cb4aCollisionMeshSoup()
{
}
void  Cb4aCollisionMeshSoup::Serialise ()
{
	planes.SerialiseHeader();
	for (uint32 i=0; i<planes.size(); ++i)
	{
		planes[i].plane.v.Serialise();
		IwSerialiseInt32(planes[i].plane.k);
		if (IwSerialiseIsReading())
			planes[i].calc = GetDistanceCalculator(planes[i].plane);
		
	}
	faces.SerialiseHeader();
	for (uint32 i=0; i<faces.size(); ++i)
	{
		IwSerialiseInt32(faces[i].start);
		IwSerialiseInt32(faces[i].num);
	}
}
b4aCollisionResult Cb4aCollisionMeshSoupFace::TraceSphere(const Cb4aCollisionMeshSoup& soup, int32 sphere, Cb4aTraceContext& context) const
{
	const Cb4aCollisionMeshSoupPlane* begin = &soup.planes[start];
	CIwVec3 shift = begin->plane.v*(-sphere);
	
	iwfixed fromDist = begin->calc(context.from+shift,begin->plane);
	if (fromDist < -b4aCollisionEpsilon)
		return COLLISION_NONE;
	iwfixed toDist = begin->calc(context.to+shift,begin->plane);
	if (toDist > b4aCollisionEpsilon)
		return COLLISION_NONE;
	if (fromDist <= toDist)
		return COLLISION_NONE;
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
	const Cb4aCollisionMeshSoupPlane* end = &soup.planes[start+num];
	do {
		++begin;
		if (begin->calc(point,begin->plane)<-b4aCollisionEpsilon-(sphere)) //-sphere makes collision be like bounding box instead of sphere. Cheap and simple trick
			return COLLISION_NONE;
	} while (begin!=end);
	context.to = point;
	begin = &soup.planes[start];
	context.collisionNormal = begin->plane.v;
	context.collisionPlaneD = begin->plane.k;
	return (fromDist>0)?COLLISION_SOMEWHERE:COLLISION_ATSTART;
}

b4aCollisionResult Cb4aCollisionMeshSoupFace::TraceLine(const Cb4aCollisionMeshSoup& soup, Cb4aTraceContext& context) const
{
	const Cb4aCollisionMeshSoupPlane* begin = &soup.planes[start];

	iwfixed fromDist = begin->calc(context.from,begin->plane);
	if (fromDist < -b4aCollisionEpsilon)
		return COLLISION_NONE;
	iwfixed toDist = begin->calc(context.to,begin->plane);
	if (toDist > b4aCollisionEpsilon)
		return COLLISION_NONE;
	if (fromDist <= toDist)
		return COLLISION_NONE;
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
	const Cb4aCollisionMeshSoupPlane* end = &soup.planes[start+num];
	do {
		++begin;
		if (begin->calc(point,begin->plane)<-b4aCollisionEpsilon)
			return COLLISION_NONE;
	} while (begin!=end);
	context.to = point;
	begin = &soup.planes[start];
	context.collisionNormal = begin->plane.v;
	context.collisionPlaneD = begin->plane.k;
	return (fromDist>0)?COLLISION_SOMEWHERE:COLLISION_ATSTART;
}
b4aCollisionResult Cb4aCollisionMeshSoup::TraceSphere(int32 sphere, Cb4aTraceContext& context) const
{
	b4aCollisionResult res = COLLISION_NONE;
	for (uint32 i=0; i<faces.size(); ++i)
	{
		b4aCollisionResult r = faces[i].TraceSphere(*this,sphere,context);
		switch (r)
		{
		case COLLISION_SOMEWHERE:
			res = COLLISION_SOMEWHERE;
			break;
		case COLLISION_ATSTART:
			return COLLISION_ATSTART;
		default:
			break;
		}
	}
	return res;
}
b4aCollisionResult Cb4aCollisionMeshSoup::TraceLine(Cb4aTraceContext& context) const 
{
	b4aCollisionResult res = COLLISION_NONE;
	for (uint32 i=0; i<faces.size(); ++i)
	{
		b4aCollisionResult r = faces[i].TraceLine(*this,context);
		switch (r)
		{
		case COLLISION_SOMEWHERE:
			res = COLLISION_SOMEWHERE;
			break;
		case COLLISION_ATSTART:
			return COLLISION_ATSTART;
		default:
			break;
		}
	}
	return res;
}
#ifdef IW_BUILD_RESOURCES

// function invoked by the text parser when parsing attributes for objects of this type
bool Cb4aCollisionMeshSoup::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
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
	if (!strcmp("num_faces", pAttrName))
	{
		int num_faces;
		pParser->ReadInt32(&num_faces);
		faces.set_capacity(num_faces);
		return true;
	}
	if (!strcmp("face", pAttrName))
	{
		faces.push_back();
		int32 planeValues[2];
		pParser->ReadInt32Array(&planeValues[0],2);
		faces.back().start = planeValues[0];
		faces.back().num = planeValues[1];
		return true;
	}
	return CIwManaged::ParseAttribute(pParser, pAttrName);
}


// function invoked by the text parser when the object definition end is encountered
void Cb4aCollisionMeshSoup::ParseClose(CIwTextParserITX* pParser)
{
	dynamic_cast<Ib4aColliderContainer*>(pParser->GetObject(-1))->AddCollider(this);
}
#endif