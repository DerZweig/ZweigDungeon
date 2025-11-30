#ifndef ZE_ENT_SHARED_H
#define ZE_ENT_SHARED_H

/**************************************************
 * Entity Interface
 **************************************************/
void Entity_Init();
void Entity_Shutdown() noexcept;
void Entity_UpdateFrame(const frame_t& frame);

#endif //ZE_ENT_SHARED_H