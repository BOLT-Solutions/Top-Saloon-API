using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TopSalon.Core.Managers;
using TopSaloon.Core.Managers;
using TopSaloon.DAL;

namespace TopSaloon.Core
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext context;
        public UnitOfWork(ApplicationDbContext context, ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            UserManager = userManager;
            RoleManager = roleManager;
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // log message and enteries
            }
            catch (DbUpdateException ex)
            {
                // log message and enteries
            }
            catch (Exception ex)
            {
                // Log here.
            }
            return false;
        }
        public ApplicationUserManager UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }

        private AdministratorsManager administratorsManager;
        public AdministratorsManager AdministratorsManager
        {
            get
            {
                if (administratorsManager == null)
                {
                    administratorsManager = new AdministratorsManager(context);
                }
                return administratorsManager;
            }
        }

        private BarbersManager barbersManager;
        public BarbersManager BarbersManager
        {
            get
            {
                if (barbersManager == null)
                {
                    barbersManager = new BarbersManager(context);
                }
                return barbersManager;
            }
        }

        private BarberProfilePhotosManager barberProfilePhotosManager;
        public BarberProfilePhotosManager BarberProfilePhotosManager
        {
            get
            {
                if (barberProfilePhotosManager == null)
                {
                    barberProfilePhotosManager = new BarberProfilePhotosManager(context);
                }
                return barberProfilePhotosManager;
            }
        }

        private BarbersQueuesManager barbersQueuesManager;
        public BarbersQueuesManager BarbersQueuesManager
        {
            get
            {
                if (barbersQueuesManager == null)
                {
                    barbersQueuesManager = new BarbersQueuesManager(context);
                }
                return barbersQueuesManager;
            }
        }

        private CustomersManager customersManager;
        public CustomersManager CustomersManager
        {
            get
            {
                if (customersManager == null)
                {
                    customersManager = new CustomersManager(context);
                }
                return customersManager;
            }
        }

        private DailyReportsManager dailyReportsManager;
        public DailyReportsManager DailyReportsManager
        {
            get
            {
                if (dailyReportsManager == null)
                {
                    dailyReportsManager = new DailyReportsManager(context);
                }
                return dailyReportsManager;
            }
        }

        private ServiceFeedBackQuestionsManager serviceFeedBackQuestionsManager;
        public ServiceFeedBackQuestionsManager ServiceFeedBackQuestionsManager
        {
            get
            {
                if (serviceFeedBackQuestionsManager == null)
                {
                    serviceFeedBackQuestionsManager = new ServiceFeedBackQuestionsManager(context);
                }
                return serviceFeedBackQuestionsManager;
            }
        }

        private OrdersManager ordersManager;
        public OrdersManager OrdersManager
        {
            get
            {
                if (ordersManager == null)
                {
                    ordersManager = new OrdersManager(context);
                }
                return ordersManager;
            }
        }

        private OrderFeedBacksManager orderFeedBacksManager;
        public OrderFeedBacksManager OrderFeedBacksManager
        {
            get
            {
                if (orderFeedBacksManager == null)
                {
                    orderFeedBacksManager = new OrderFeedBacksManager(context);
                }
                return orderFeedBacksManager;
            }
        }

        private OrderFeedBackQuestionsManager orderFeedBackQuestionsManager;
        public OrderFeedBackQuestionsManager OrderFeedBackQuestionsManager
        {
            get
            {
                if (orderFeedBackQuestionsManager == null)
                {
                    orderFeedBackQuestionsManager = new OrderFeedBackQuestionsManager(context);
                }
                return orderFeedBackQuestionsManager;
            }
        }

        private OrderServicesManager orderServicesManager;
        public OrderServicesManager OrderServicesManager
        {
            get
            {
                if (orderServicesManager == null)
                {
                    orderServicesManager = new OrderServicesManager(context);
                }
                return orderServicesManager;
            }
        }

        private ServicesManager servicesManager;
        public ServicesManager ServicesManager
        {
            get
            {
                if (servicesManager == null)
                {
                    servicesManager = new ServicesManager(context);
                }
                return servicesManager;
            }
        }

        private ShopsManager shopsManager;
        public ShopsManager ShopsManager
        {
            get
            {
                if (shopsManager == null)
                {
                    shopsManager = new ShopsManager(context);
                }
                return shopsManager;
            }
        }

        private SMSManager sMSManager;
        public SMSManager SMSManager
        {
            get
            {
                if (sMSManager == null)
                {
                    sMSManager = new SMSManager(context);
                }
                return sMSManager;
            }
        }

        private CompleteOrdersManager completeOrdersManager;
        public CompleteOrdersManager CompleteOrdersManager
        {
            get
            {
                if (completeOrdersManager == null)
                {
                    completeOrdersManager = new CompleteOrdersManager(context);
                }
                return completeOrdersManager;
            }
        }

        private BarberLoginsManager barberLoginsManager;
        public BarberLoginsManager BarberLoginsManager
        {
            get
            {
                if (barberLoginsManager == null)
                {
                    barberLoginsManager = new BarberLoginsManager(context);
                }
                return barberLoginsManager;
            }
        }

        private GuestNumberManager guestNumberManager;
        public GuestNumberManager GuestNumberManager
        {
            get
            {
                if (guestNumberManager == null)
                {
                    guestNumberManager = new GuestNumberManager(context);
                }
                return guestNumberManager;
            }
        }

        private PromoCodeManager promoCodeManager;
        public PromoCodeManager PromoCodeManager
        {
            get
            {
                if (promoCodeManager == null)
                {
                    promoCodeManager = new PromoCodeManager(context);
                }
                return promoCodeManager;
            }
        }

    }
    
}
