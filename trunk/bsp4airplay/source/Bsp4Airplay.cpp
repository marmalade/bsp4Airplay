#include "Bsp4Airplay.h"
#include "b4aEntity.h"
#include "b4aCollisionMeshSoup.h"
#include "b4aCollisionConvexBrush.h"

void Bsp4Airplay::Bsp4AirpayInit()
{
	#ifdef IW_BUILD_RESOURCES
	//IwGetResManager()->AddHandler(new CResHandlerWAV);

	IwClassFactoryAdd("Cb4aLeaf", Cb4aLeafFactory, 0);
	IwClassFactoryAdd("Cb4aNode", Cb4aNodeFactory, 0);
	IwClassFactoryAdd("Cb4aEntity", Cb4aEntityFactory, 0);
	IwClassFactoryAdd("Cb4aLevelVBSubcluster", Cb4aLevelVBSubclusterFactory, 0);
	IwClassFactoryAdd("Cb4aLevelVertexBuffer", Cb4aLevelVertexBufferFactory, 0);
	IwClassFactoryAdd("Cb4aLevelMaterial", Cb4aLevelMaterialFactory, 0);
	#endif

	IW_CLASS_REGISTER(Cb4aLevel);
	IW_CLASS_REGISTER(Cb4aCollisionMeshSoup);
	IW_CLASS_REGISTER(Cb4aCollisionConvexBrush);
}
void Bsp4Airplay::Bsp4AirpayTerminate()
{
}