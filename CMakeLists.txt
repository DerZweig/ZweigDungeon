cmake_minimum_required(VERSION 3.25)

project (ZweigDungeon)
set(APP_BINARY ZweigDungeon)
set(SOURCE_DIR ${CMAKE_SOURCE_DIR}/src)
set(LIBRARY_DIR ${CMAKE_SOURCE_DIR}/thirdparty)

set(SOURCES_FILES
        "${SOURCE_DIR}/application.cpp" 
        "${SOURCE_DIR}/sdl/sdl_main.cpp"
        "${SOURCE_DIR}/sdl/sdl_input.cpp"
        "${SOURCE_DIR}/sdl/sdl_display.cpp"
        "${SOURCE_DIR}/sdl/sdl_sound.cpp"
        "${SOURCE_DIR}/common/com_main.cpp"
        "${SOURCE_DIR}/common/com_logger.cpp"
        "${SOURCE_DIR}/common/com_config.cpp"
        "${SOURCE_DIR}/common/com_files.cpp"
        "${SOURCE_DIR}/common/com_timer.cpp"
        "${SOURCE_DIR}/video/vid_main.cpp"
        "${SOURCE_DIR}/video/vid_buffer.cpp"
        "${SOURCE_DIR}/video/vid_font.cpp"
        "${SOURCE_DIR}/video/vid_entity.cpp"
        "${SOURCE_DIR}/video/vid_effects.cpp"
        "${SOURCE_DIR}/sound/snd_main.cpp"
        "${SOURCE_DIR}/sound/snd_entity.cpp"
        "${SOURCE_DIR}/sound/snd_effects.cpp"
        "${SOURCE_DIR}/ui/ui_main.cpp"
        "${SOURCE_DIR}/entity/ent_main.cpp")


IF (WIN32)
    list(APPEND EXECUTABLE_OPTIONS "WIN32")
    list(APPEND STATIC_LIBRARIES kernel32.lib user32.lib gdi32.lib ole32.lib)

    if(MSVC)
        list(APPEND ADDITIONAL_CPP_FLAGS "/DWIN32=1 /D_WINDOWS /DUNICODE=1 /D_CRT_SECURE_NO_WARNINGS=1 /W3 /EHsc /fp:strict")

        list(APPEND ADDITIONAL_INCLUDE_DIRS "${LIBRARY_DIR}/SDL3-3.2.26/include")
        list(APPEND STATIC_LIBRARIES "${LIBRARY_DIR}/SDL3-3.2.26/lib/win64/SDL3.lib")
        list(APPEND COPY_TO_OUTPUT "${LIBRARY_DIR}/SDL3-3.2.26/lib/win64/SDL3.dll")
    endif()
ENDIF (WIN32)


set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_FLAGS "${CXX_FLAGS} ${ADDITIONAL_CPP_FLAGS}")
set(EXECUTABLE_OUTPUT_PATH ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)

add_executable(${APP_BINARY} ${EXECUTABLE_OPTIONS} ${SOURCES_FILES})
target_link_libraries(${APP_BINARY} ${STATIC_LIBRARIES})
target_include_directories(${APP_BINARY} PRIVATE ${ADDITIONAL_INCLUDE_DIRS})

foreach(LIBRARY IN LISTS COPY_TO_OUTPUT)
    add_custom_command(TARGET ${APP_BINARY} POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy ${LIBRARY} $<TARGET_FILE_DIR:${APP_BINARY}>)
endforeach()