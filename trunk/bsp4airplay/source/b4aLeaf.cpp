#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include <IwModelBlock.h>
#include "b4aLeaf.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLeaf g_parseLeaf;
#endif
}

//Constructor
Cb4aLeaf::Cb4aLeaf()
{
	lastVisibleOnFrame = 0;
}

//Desctructor
Cb4aLeaf::~Cb4aLeaf()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aLeaf::Serialise ()
{
	bbox.Serialise();
	if (IwSerialiseIsReading())
	{
		sphere.t.x = (bbox.m_Min.x+bbox.m_Max.x)/2;
		sphere.t.y = (bbox.m_Min.y+bbox.m_Max.y)/2;
		sphere.t.z = (bbox.m_Min.z+bbox.m_Max.z)/2;
		sphere.SetRadius((bbox.m_Max-sphere.t).GetLength());
	}
	visible_clusters.SerialiseHeader();
	for (uint32 i=0; i<visible_clusters.size(); ++i)
		IwSerialiseInt32(visible_clusters[i]);
	visible_leaves.SerialiseHeader();
	for (uint32 i=0; i<visible_leaves.size(); ++i)
		IwSerialiseInt32(visible_leaves[i]);
	colliders.Serialise();
	
}
void Cb4aLeaf::RenderProjection(Cb4aLevel* l,CIwTexture* tex, const CIwMat& view, const CIwVec3& whz)
{

	//l->RenderCluster(cluster);
}
void Cb4aLeaf::AddCollider(Ib4aCollider* c)
{
	colliders.push_back(c);
}

void Cb4aLeaf::Render(Cb4aLevel*l)
{
	for (CIwArray<int32>::iterator i=visible_clusters.begin(); i!=visible_clusters.end();++i)
		l->RenderCluster(*i);
}
b4aCollisionResult Cb4aLeaf::TraceSphere(const Cb4aLevel*l, int32 sphere, Cb4aTraceContext& context) const
{
	b4aCollisionResult res = COLLISION_NONE;
	for (Cb4aColliderList::const_iterator i=colliders.begin(); i!=colliders.end(); ++i)
	{
		b4aCollisionResult r = (*i)->TraceSphere(sphere, context);
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

b4aCollisionResult Cb4aLeaf::TraceLine(const Cb4aLevel*l, Cb4aTraceContext& context) const
{
	b4aCollisionResult res = COLLISION_NONE;
	for (Cb4aColliderList::const_iterator i=colliders.begin(); i!=colliders.end(); ++i)
	{
		b4aCollisionResult r = (*i)->TraceLine(context);
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
void* Bsp4Airplay::Cb4aLeafFactory()
{
	return &g_parseLeaf;
}

// Parse from text file: start block.
void  ParseLeaf::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateLeaf();
}
void ParseLeaf::AddCollider(Ib4aCollider* c)
{
	_this->colliders.push_back(c);
}
// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLeaf::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_visible_leaves", pAttrName))
	{
		int32 n;
		pParser->ReadInt32(&n);
		_this->visible_leaves.resize(n);
		return true;
	}
	if (!strcmp("num_visible_clusters", pAttrName))
	{
		int32 n;
		pParser->ReadInt32(&n);
		_this->visible_clusters.resize(n);
		return true;
	}
	if (!strcmp("visible_leaves", pAttrName))
	{
		for (uint32 i=0; i<_this->visible_leaves.size(); ++i)
		pParser->ReadInt32(&_this->visible_leaves[i]);
		return true;
	}
	if (!strcmp("visible_clusters", pAttrName))
	{
		for (uint32 i=0; i<_this->visible_clusters.size(); ++i)
		pParser->ReadInt32(&_this->visible_clusters[i]);
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
void ParseLeaf::ParseClose(CIwTextParserITX* pParser)
{
}
#endif