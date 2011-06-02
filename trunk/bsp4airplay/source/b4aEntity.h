#pragma once
#include <string>
#include <IwResource.h>
#include <IwGx.h>

namespace Bsp4Airplay
{
	class Cb4aEntity;

#ifdef IW_BUILD_RESOURCES
	void* Cb4aEntityFactory();

	class ParseEntity: public CIwManaged
	{
		Cb4aEntity* _this;
		public:
		// ---- Text resources ----
		// Parse from text file: start block.
		virtual void  ParseOpen (CIwTextParserITX *pParser);

		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
	};
#endif

	class Cb4aEntity
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseEntity;
#endif
		std::string classname;
		CIwVec3 origin;
				
	public:

		//Constructor
		Cb4aEntity();
		//Desctructor
		~Cb4aEntity();

		void  Serialise ();

		inline const std::string & GetClassName() const {return classname;}
		inline const CIwVec3 & GetOrigin() const {return origin;}
	};

}