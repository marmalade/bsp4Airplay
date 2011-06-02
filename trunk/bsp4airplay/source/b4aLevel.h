#pragma once
#include <IwResource.h>
#include "b4aLeaf.h"
#include "b4aNode.h"
#include "b4aEntity.h"
#include "b4aLevelVB.h"
#include "b4aLevelVBSubcluster.h"
#include "b4aLevelMaterial.h"
#include "Ib4aProjection.h"

#define BSP4AIRPLAY_RESTYPE_LEVEL	"Cb4aLevel"

namespace Bsp4Airplay
{


	class Cb4aLevel : public CIwResource
	{
		int32 frameid;
		CIwArray<Cb4aLevelVertexBuffer> buffers;
		CIwArray<Cb4aLeaf> leaves;
		CIwArray<Cb4aNode> nodes;
		CIwArray<Cb4aCollisionMeshSoupPlane> planes;
		CIwArray<Cb4aEntity> entities;
		CIwArray<Cb4aLevelMaterial> materials;
		CIwArray<Cb4aLevelVBSubcluster> clusters;
		uint32 defaultTextureHash;
		CIwBBox visibleArea;
	public:
		//Declare managed class
		IW_MANAGED_DECLARE(Cb4aLevel);

		//Constructor
		Cb4aLevel();
		//Desctructor
		virtual ~Cb4aLevel();

		virtual void  Serialise ();

		void BeginRender();
		void BeginRender(const CIwVec3 & viewer);
		void RenderProjection(Ib4aProjection* proj);
		void RenderCluster(int32 i);

		void EndRender();

		inline void BindMaterial(uint32 i) {materials[i].Bind(this);};

		bool TraceLine(Cb4aTraceContext& context) const;
		bool TraceSphere(int32 r, Cb4aTraceContext& context) const;
		inline const Cb4aNode& GetNode(uint32 i) const { return nodes[i];}
		inline const Cb4aCollisionMeshSoupPlane& GetPlane(uint32 i) const { return planes[i];}
		inline const Cb4aLeaf& GetLeaf(uint32 i) const { return leaves[i];}
		inline bool IsInVisibleArea(const CIwVec3 & v) const { return visibleArea.ContainsVec(v);}
		int FindEntityByClassName(const char* name,int startFrom=0) const;
		inline uint32 GetNumEntities() const {return entities.size();}
		inline const Cb4aEntity* GetEntityAt(uint32 i) const {return &entities[i];}
		inline int32 GetCurrentFrameId() const {return frameid;}
		CIwTexture* GetDefaultTextrure();
		
		void ScheduleRender(int32 i, Cb4aLevelVBSubcluster*);
	protected:
		void ResetFrameCounter();
		// ---- Text resources ----
#ifdef IW_BUILD_RESOURCES
	public:
		Cb4aLeaf* AllocateLeaf();
		Cb4aNode* AllocateNode();
		Cb4aEntity* AllocateEntity();
		Cb4aLevelVBSubcluster* AllocateCluster();
		Cb4aLevelVertexBuffer* AllocateLevelVertexBuffer();
		Cb4aLevelMaterial* AllocateLevelMaterial();
		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
#endif
	};
}