import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';

export default function MainLayout({ children }) {
  return (
    <div className="relative z-10 flex h-full grow flex-col">
      <div className="mx-auto flex w-full max-w-6xl flex-1 flex-col px-4 sm:px-6 lg:px-8">
        <Navbar />
        {children}
        <Footer />
      </div>
    </div>
  );
}
