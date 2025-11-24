#ifndef ZE_ENTITIES_H
#define ZE_ENTITIES_H

#include "common.h"

/**************************************************
 * Entity Manager Class
 **************************************************/
struct EntityManager : virtual Common
{
        EntityManager()                                = default;
        EntityManager(EntityManager&&)                 = delete;
        EntityManager(const EntityManager&)            = delete;
        EntityManager& operator=(EntityManager&&)      = delete;
        EntityManager& operator=(const EntityManager&) = delete;
        ~EntityManager() noexcept override;

        virtual void UpdateEntities();
};

#endif //ZE_ENTITIES_H
