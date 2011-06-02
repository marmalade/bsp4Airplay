#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aLevel.h"
#include "b4aNode.h"
#include "Bsp4Airplay.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseNode g_parseNode;
#endif
	
}

//Constructor
Cb4aNode::Cb4aNode()
{
}

//Desctructor
Cb4aNode::~Cb4aNode()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aNode::Serialise ()
{
	bbox.Serialise();
	IwSerialiseInt32(plane);
	IwSerialiseBool(is_front_leaf);
	IwSerialiseInt32(front);
	IwSerialiseBool(is_back_leaf);
	IwSerialiseInt32(back);
}
bool Cb4aNode::WalkNode(const Cb4aLevel*l, const CIwVec3 & viewer, int32* nextNode) const
{
	const Cb4aCollisionMeshSoupPlane& planecalc = l->GetPlane(this->plane);
	int32 dist = planecalc.Calculate(viewer);
	bool positive = dist>=0;
	if (positive)
	{
		*nextNode = front;
		return is_front_leaf;
	}
	else
	{
		*nextNode = back;
		return is_back_leaf;
	}
}
b4aCollisionResult Cb4aNode::TraceFrontLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	if (is_front_leaf)
		return l->GetLeaf(front).TraceLine(l,context);
	return l->GetNode(front).TraceLine(l,context);
}
b4aCollisionResult Cb4aNode::TraceBackLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	if (is_back_leaf)
		return l->GetLeaf(back).TraceLine(l,context);
	return l->GetNode(back).TraceLine(l,context);
}
b4aCollisionResult Cb4aNode::TraceFrontSphere(const Cb4aLevel*l, int32 sphere, Cb4aTraceContext& context) const
{
	if (is_front_leaf)
		return l->GetLeaf(front).TraceSphere(l,sphere,context);
	return l->GetNode(front).TraceSphere(l,sphere,context);
}
b4aCollisionResult Cb4aNode::TraceBackSphere(const Cb4aLevel*l, int32 sphere, Cb4aTraceContext& context) const
{
	if (is_back_leaf)
		return l->GetLeaf(back).TraceSphere(l,sphere,context);
	return l->GetNode(back).TraceSphere(l,sphere,context);
}
b4aCollisionResult Cb4aNode::TraceSphere(const Cb4aLevel*l, int32 sphere, Cb4aTraceContext& context) const
{
	const Cb4aCollisionMeshSoupPlane& planecalc = l->GetPlane(this->plane);
	iwfixed fromDist = planecalc.Calculate(context.from);
	iwfixed toDist = planecalc.Calculate(context.to);
	int32 r = sphere<<IW_GEOM_POINT;
	if (fromDist >= r && toDist >= r)
		return TraceFrontSphere(l,sphere,context);
	if (fromDist < -r && toDist < -r)
		return TraceBackSphere(l,sphere,context);

	if (fromDist >= toDist)
	{
		b4aCollisionResult res = TraceFrontSphere(l,sphere,context);
		switch (res)
		{
		case COLLISION_ATSTART:
			return COLLISION_ATSTART;
		case COLLISION_SOMEWHERE:
			toDist = planecalc.Calculate(context.to);
			if (toDist >= r) return res;
		default:
			return (b4aCollisionResult)((int)res | (int)TraceBackSphere(l,sphere,context));
		}
	}
	b4aCollisionResult res = TraceBackSphere(l,sphere,context);
	switch (res)
	{
	case COLLISION_ATSTART:
		return COLLISION_ATSTART;
	case COLLISION_SOMEWHERE:
		toDist = planecalc.Calculate(context.to);
		if (toDist <= -r) return res;
	default:
		return (b4aCollisionResult)((int)res | (int)TraceFrontSphere(l,sphere,context));
	}
	return res;
}
b4aCollisionResult Cb4aNode::TraceLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	const Cb4aCollisionMeshSoupPlane& planecalc = l->GetPlane(this->plane);
	iwfixed fromDist = planecalc.Calculate(context.from);
	iwfixed toDist = planecalc.Calculate(context.to);
	if (fromDist >= 0 && toDist >= 0)
		return TraceFrontLine(l,context);
	if (fromDist < 0 && toDist < 0)
		return TraceBackLine(l,context);

	if (fromDist >= 0)
	{
		b4aCollisionResult res = TraceFrontLine(l,context);
		if (res) return res;
		return TraceBackLine(l,context);
	}
	b4aCollisionResult res = TraceBackLine(l,context);
	if (res) return res;
	return TraceFrontLine(l,context);
}
#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aNodeFactory()
{
	return &g_parseNode;
}

// Parse from text file: start block.
void  ParseNode::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateNode();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseNode::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("plane", pAttrName))
	{
		pParser->ReadInt32(&_this->plane);
		//iwfixed planeValues[4];
		//pParser->ReadInt32Array(&planeValues[0],4);
		//_this->plane = Cb4aPlane(CIwVec3(planeValues[0],planeValues[1],planeValues[2]),planeValues[3]);
		return true;
	}
	if (!strcmp("is_front_leaf", pAttrName))
	{
		pParser->ReadBool(&_this->is_front_leaf);
		return true;
	}
	if (!strcmp("front", pAttrName))
	{
		pParser->ReadInt32(&_this->front);
		return true;
	}
	if (!strcmp("is_back_leaf", pAttrName))
	{
		pParser->ReadBool(&_this->is_back_leaf);
		return true;
	}
	if (!strcmp("back", pAttrName))
	{
		pParser->ReadInt32(&_this->back);
		return true;
	}
	if (!strcmp("mins", pAttrName))
	{
		pParser->ReadInt32Array(&_this->bbox.m_Min.x,3);
		return true;
	}
	if (!strcmp("maxs", pAttrName))
	{
		pParser->ReadInt32Array(&_this->bbox.m_Max.x,3);
		return true;
	}

	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseNode::ParseClose(CIwTextParserITX* pParser)
{
}

#endif