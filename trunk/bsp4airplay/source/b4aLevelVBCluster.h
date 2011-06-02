#pragma once
#include <IwResource.h>
#include <b4aLevelVBSubcluster.h>

namespace Bsp4Airplay
{
	class Cb4aLevelVBCluster;
	class Cb4aLevel;
#ifdef IW_BUILD_RESOURCES
	void* Cb4aLevelVBClusterFactory();

	class ParseLevelVBCluster: public CIwManaged
	{
		Cb4aLevelVBCluster* _this;
		public:
		// ---- Text resources ----
		// Parse from text file: start block.
		virtual void  ParseOpen (CIwTextParserITX *pParser);

		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);

		Cb4aLevelVBSubcluster* AllocateSubcluster();
	};
#endif

	class Cb4aLevelVBCluster
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseLevelVBCluster;
#endif
	public:
		CIwArray<Cb4aLevelVBSubcluster> subclusters;
		uint32 vertexbuffer;
		uint32 lastVisibleOnFrame;
	public:

		//Constructor
		Cb4aLevelVBCluster();
		//Desctructor
		~Cb4aLevelVBCluster();

		void  Serialise ();

		void Render(Cb4aLevel*);


	};

}