import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';

export default function MainLayout({ children }) {
  return (
    <div className="relative z-10 flex h-full grow flex-col">
      <Navbar />
      <div className="mx-auto flex w-full flex-1 flex-col pt-2 bg-transparent text-gray-900 dark:text-white transition-colors duration-200">
        {children}
        <div className="w-full px-4 sm:px-6 lg:px-12 xl:px-24 bg-card-light dark:bg-card-darker/50 backdrop-blur-sm border-t border-white/5 mt-auto">
          <Footer />
        </div>
      </div>
    </div>
  );
}
