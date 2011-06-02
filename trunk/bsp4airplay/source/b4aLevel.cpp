#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aLevel.h"

using namespace Bsp4Airplay;

IW_CLASS_FACTORY(Cb4aLevel);
IW_MANAGED_IMPLEMENT(Cb4aLevel);

namespace Bsp4Airplay
{
}

//Constructor
Cb4aLevel::Cb4aLevel()
{
	defaultTextureHash = IwHashString("checkers");
	frameid = 0;
}

//Desctructor
Cb4aLevel::~Cb4aLevel()
{
}

void  Cb4aLevel::Serialise ()
{
	CIwResource::Serialise();

	IwSerialiseUInt32(defaultTextureHash);

	materials.SerialiseHeader();
	for (uint32 i=0; i<materials.size(); ++i)
		materials[i].Serialise();

	planes.SerialiseHeader();
	for (uint32 i=0; i<planes.size(); ++i)
	{
		Cb4aPlane& plane = planes[i].plane;
		plane.v.Serialise();
		IwSerialiseInt32(plane.k);
		if (IwSerialiseIsReading())
			planes[i].calc = GetDistanceCalculator(plane);
	}

	buffers.SerialiseHeader();
	for (uint32 i=0; i<buffers.size(); ++i)
		buffers[i].Serialise();

	clusters.SerialiseHeader();
	for (uint32 i=0; i<clusters.size(); ++i)
		clusters[i].Serialise();

	leaves.SerialiseHeader();
	for (uint32 i=0; i<leaves.size(); ++i)
		leaves[i].Serialise();

	nodes.SerialiseHeader();
	for (uint32 i=0; i<nodes.size(); ++i)
		nodes[i].Serialise();

	entities.SerialiseHeader();
	for (uint32 i=0; i<entities.size(); ++i)
		entities[i].Serialise();
}
CIwTexture* Cb4aLevel::GetDefaultTextrure()
{
	return (CIwTexture*)IwGetResManager()->GetResHashed(defaultTextureHash, "CIwTexture", IW_RES_PERMIT_NULL_F | IW_RES_SEARCH_ALL_F);
}
int Cb4aLevel::FindEntityByClassName(const char* name, int startFrom) const
{
	for (uint32 i=startFrom; i<entities.size(); ++i)
		if (entities[i].GetClassName() == name)
			return (int)i;
	return -1;
}
void Cb4aLevel::RenderCluster(int32 i)
{
	if (i < 0) return;
	Cb4aLevelVBSubcluster& cluster = clusters[i];
	if (cluster.IsVisible())
		ScheduleRender(cluster.vb, &cluster);
}
void Cb4aLevel::ResetFrameCounter()
{
	frameid = 1;
	for (uint32 i=0; i<leaves.size(); ++i)
		leaves[i].SetVisible(0);
}
void Cb4aLevel::BeginRender(const CIwVec3 & viewer)
{

	++frameid;
	if (frameid == 0x7FFFFFFF)
		ResetFrameCounter();
	CIwMat modelMatrix;
	modelMatrix.SetIdentity();

	IwGxSetModelMatrix(&modelMatrix);
	IwGxLightingOff();

	for (uint32 i=0;i<buffers.size(); ++i)
		buffers[i].ClearQueue();

	int node = 0;
	while (!nodes[node].WalkNode(this, viewer, &node));

	Cb4aLeaf* currentLeaf = &leaves[node];
	visibleArea = currentLeaf->GetBBox();
	currentLeaf->SetVisible(frameid);

	for (uint32 i=0;i<currentLeaf->visible_leaves.size(); ++i)
	{
		Cb4aLeaf* visibleLeaf = &leaves[currentLeaf->visible_leaves[i]];
		if (!IwGxClipSphere(visibleLeaf->GetSphere()))
		{
			const CIwBBox & b = visibleLeaf->GetBBox();
			visibleArea.BoundVec(&b.m_Min);
			visibleArea.BoundVec(&b.m_Max);
			visibleLeaf->SetVisible(frameid);
		}
	}
	int32 farZ = (visibleArea.m_Max-visibleArea.m_Min).GetLengthSafe();
	if (farZ < 16) farZ = 16;
	IwGxSetFarZNearZ(farZ,8);

	currentLeaf->Render(this);

	for (uint32 i=0;i<buffers.size(); ++i)
		buffers[i].Flush(this);
}
void Cb4aLevel::RenderProjection(Ib4aProjection* proj)
{
	for (uint32 i=0; i<buffers.size(); ++i)
	{
		for (uint32 j=0; j<buffers[i].renderQueue.size(); ++j)
		{
			proj->Add(this,&buffers[i], buffers[i].renderQueue[j]);
		}
	}
	proj->Flush();
}

void Cb4aLevel::EndRender()
{
}
void ScheduleRender(int32 i, Cb4aLevelVBSubcluster*)
{
}
void Cb4aLevel::ScheduleRender(int32 i, Cb4aLevelVBSubcluster*c)
{
	buffers[i].ScheduleCluster(c);
}
void Cb4aLevel::BeginRender()
{
	const CIwMat& m = IwGxGetViewMatrix();
	BeginRender(CIwVec3(m.t.x<<IW_GEOM_POINT,m.t.y<<IW_GEOM_POINT,m.t.z<<IW_GEOM_POINT));
}
bool Cb4aLevel::TraceLine(Cb4aTraceContext& context) const
{
	if (nodes.empty())
		return false;
	return nodes[0].TraceLine(this, context) != COLLISION_NONE;
}
bool Cb4aLevel::TraceSphere(int32 r, Cb4aTraceContext& context) const
{
	if (nodes.empty())
		return false;
	return nodes[0].TraceSphere(this, r, context) != COLLISION_NONE;
}
#ifdef IW_BUILD_RESOURCES
Cb4aLeaf* Cb4aLevel::AllocateLeaf()
{
	leaves.push_back();
	return &leaves.back();
}
Cb4aNode* Cb4aLevel::AllocateNode()
{
	nodes.push_back();
	return &nodes.back();
}
Cb4aEntity* Cb4aLevel::AllocateEntity()
{
	entities.push_back();
	return &entities.back();
}
Cb4aLevelVBSubcluster* Cb4aLevel::AllocateCluster()
{
	clusters.push_back();
	return &clusters.back();
}
Cb4aLevelVertexBuffer* Cb4aLevel::AllocateLevelVertexBuffer()
{
	buffers.push_back();
	return &buffers.back();
}
Cb4aLevelMaterial* Cb4aLevel::AllocateLevelMaterial()
{
	materials.push_back();
	return &materials.back();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool Cb4aLevel::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_leaves", pAttrName))
	{
		int num_leaves;
		pParser->ReadInt32(&num_leaves);
		leaves.set_capacity(num_leaves);
		return true;
	}
	if (!strcmp("num_nodes", pAttrName))
	{
		int num_nodes;
		pParser->ReadInt32(&num_nodes);
		nodes.set_capacity(num_nodes);
		return true;
	}
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
	if (!strcmp("num_entities", pAttrName))
	{
		int num_nodes;
		pParser->ReadInt32(&num_nodes);
		entities.set_capacity(num_nodes);
		return true;
	}
	if (!strcmp("num_materials", pAttrName))
	{
		int num_materials;
		pParser->ReadInt32(&num_materials);
		materials.set_capacity(num_materials);
		return true;
	}
	if (!strcmp("num_clusters", pAttrName))
	{
		uint32 num_clusters;
		pParser->ReadUInt32(&num_clusters);
		clusters.set_capacity(num_clusters);
		return true;
	}
	if (!strcmp("num_vbs", pAttrName))
	{
		uint32 num_vbs;
		pParser->ReadUInt32(&num_vbs);
		buffers.set_capacity(num_vbs);
		return true;
	}
	
	
	return CIwResource::ParseAttribute(pParser, pAttrName);
}


// function invoked by the text parser when the object definition end is encountered
void Cb4aLevel::ParseClose(CIwTextParserITX* pParser)
{
	// Return value to resource Build() method
	pParser->SetReturnValue(this);
	CIwResManager* manager = IwGetResManager();
	CIwResGroup* group = manager->GetCurrentGroup();
	if (group)
		group->AddRes(BSP4AIRPLAY_RESTYPE_LEVEL, this);
}
#endif