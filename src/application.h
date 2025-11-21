#ifndef ZE_APPLICATION_H
#define ZE_APPLICATION_H

struct IApplication
{
protected:
        IApplication() noexcept  = default;
        ~IApplication() noexcept = default;

public:
        IApplication(IApplication&&)                 = delete;
        IApplication(const IApplication&)            = delete;
        IApplication& operator=(IApplication&&)      = delete;
        IApplication& operator=(const IApplication&) = delete;

        virtual bool init(int argc, char** argv) = 0;
        virtual void run() = 0;
};

extern IApplication& app;

#endif //ZE_APPLICATION_H
