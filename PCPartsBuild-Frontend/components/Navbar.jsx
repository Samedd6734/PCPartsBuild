"use client";
import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { motion, AnimatePresence } from 'framer-motion';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

export default function Navbar() {
  const [lang, setLang] = useState('tr');
  const [user, setUser] = useState(null);
  const [scrollY, setScrollY] = useState(0);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isProfileDropdownOpen, setIsProfileDropdownOpen] = useState(false);
  const [isLangDropdownOpen, setIsLangDropdownOpen] = useState(false);

  useEffect(() => {
    const savedLang = localStorage.getItem('lang') || 'tr';
    setLang(savedLang);
    const loggedInUser = localStorage.getItem('loggedInUser');
    if (loggedInUser) setUser(loggedInUser);
    
    const handleScroll = () => {
      setScrollY(window.scrollY);
    };
    const handleStorageChange = () => {
      setLang(localStorage.getItem('lang') || 'tr');
      setUser(localStorage.getItem('loggedInUser'));
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    window.addEventListener('storage', handleStorageChange);
    return () => {
      window.removeEventListener('scroll', handleScroll);
      window.removeEventListener('storage', handleStorageChange);
    };
  }, []);

  // Calculate dynamic styles based on scroll depth (0 to 200px)
  const scrollProgress = Math.min(scrollY / 200, 1);
  const navOpacity = 0.3 + (scrollProgress * 0.65); // 0.3 to 0.95
  const navBlur = 4 + (scrollProgress * 12); // backdrop-blur-sm (4px) to backdrop-blur-lg (16px)
  const navPadding = 20 - (scrollProgress * 8); // 20px (py-5) to 12px (py-3)
  const navBorderColor = `rgba(255, 255, 255, ${0.02 + (scrollProgress * 0.08)})`; // thin to 0.1 opacity
  const navShadowOpacity = scrollProgress * 0.4; // subtle shadow fade in

  const navStyle = {
    backgroundColor: `rgba(13, 17, 23, ${navOpacity})`,
    backdropFilter: `blur(${navBlur}px)`,
    WebkitBackdropFilter: `blur(${navBlur}px)`,
    paddingTop: `${navPadding}px`,
    paddingBottom: `${navPadding}px`,
    borderColor: navBorderColor,
    boxShadow: `0 10px 30px -10px rgba(0, 0, 0, ${navShadowOpacity})`,
    transition: 'background-color 0.3s ease, padding 0.3s ease, backdrop-filter 0.3s ease'
  };

  const toggleLang = (targetLang) => {
    localStorage.setItem('lang', targetLang);
    setLang(targetLang);
    setIsLangDropdownOpen(false);
    window.dispatchEvent(new Event('storage'));
    window.location.reload();
  };

  const handleLogout = (e) => {
    e.preventDefault();
    Swal.fire({
      title: lang === 'tr' ? 'Oturum Kapatılıyor' : 'Logging Out',
      text: lang === 'tr' ? 'Güvenli bir şekilde çıkış yapılıyor...' : 'Logging out securely...',
      icon: 'info',
      background: '#0d1117',
      color: '#fff',
      showConfirmButton: false,
      timer: 1000
    }).then(() => {
      localStorage.removeItem('token');
      localStorage.removeItem('loggedInUser');
      setUser(null);
      window.location.reload();
    });
  };

  const t = translations[lang];

  // Professional, Precise Spring for UI Elements
  const transition = { type: "spring", stiffness: 400, damping: 30 };

  return (
    <nav className="fixed top-0 left-0 right-0 z-[100] border-b" style={navStyle}>
      <div className="container mx-auto px-6 lg:px-12 flex items-center justify-between">
        
        {/* Logo - Industrial & Precise */}
        <div className="flex items-center">
          <Link href="/" className="flex items-center gap-2 group transition-opacity hover:opacity-90">
            <img 
              src="/Images/logo.webp" 
              alt="PCPartsBuild" 
              className="h-10 w-auto object-contain" 
            />
          </Link>
        </div>

        {/* Navigation - Clean Tech Typography */}
        <div className="hidden lg:flex items-center gap-10">
          {[
            { href: '/pts', label: t["nav-wizard"] },
            { href: '/puk', label: t["nav-compatibility"] },
            { href: '/pbg', label: t["nav-info"] }
          ].map((link) => (
            <Link 
              key={link.href}
              href={link.href} 
              className="relative text-xs font-black uppercase tracking-[0.2em] text-white/50 transition-all hover:text-primary group"
            >
              {link.label}
              <span className="absolute -bottom-2 left-0 w-0 h-0.5 bg-primary transition-all group-hover:w-full"></span>
            </Link>
          ))}
        </div>

        {/* Tech Controls */}
        <div className="flex items-center gap-4">
          
          {/* Lang Selector */}
          <div className="relative hidden sm:block">
            <button 
              onClick={() => setIsLangDropdownOpen(!isLangDropdownOpen)}
              className="w-10 h-10 flex items-center justify-center rounded-lg border border-white/5 text-white/40 hover:text-white hover:bg-white/5 transition-all text-xs font-black"
            >
              {lang.toUpperCase()}
            </button>
            <AnimatePresence>
              {isLangDropdownOpen && (
                <motion.div 
                  initial={{ opacity: 0, scale: 0.95, y: 5 }}
                  animate={{ opacity: 1, scale: 1, y: 0 }}
                  exit={{ opacity: 0, scale: 0.95, y: 5 }}
                  transition={{ duration: 0.2 }}
                  className="absolute right-0 top-full mt-3 w-32 bg-card-darker border border-white/10 rounded-lg shadow-2xl overflow-hidden"
                >
                  <button onClick={() => toggleLang('tr')} className={`w-full px-4 py-3 text-left text-[10px] font-black tracking-widest uppercase hover:bg-primary hover:text-white transition-colors ${lang === 'tr' ? 'text-primary' : 'text-white/60'}`}>Türkçe</button>
                  <button onClick={() => toggleLang('en')} className={`w-full px-4 py-3 text-left text-[10px] font-black tracking-widest uppercase hover:bg-primary hover:text-white transition-colors ${lang === 'en' ? 'text-primary' : 'text-white/60'}`}>English</button>
                </motion.div>
              )}
            </AnimatePresence>
          </div>

          {/* User Status */}
          {!user ? (
            <div className="flex items-center gap-4">
              <Link href="/giris" className="hidden sm:block text-[10px] font-black uppercase tracking-[0.2em] text-white/40 hover:text-white transition-colors">
                {t["auth-login"]}
              </Link>
              <Link href="/kaydol" className="h-10 px-6 flex items-center justify-center bg-primary text-white text-[10px] font-black uppercase tracking-[0.2em] rounded-md transition-all hover:brightness-110 active:scale-95 shadow-lg shadow-primary/10">
                {t["auth-signup"]}
              </Link>
            </div>
          ) : (
            <div className="relative">
              <button 
                onClick={() => setIsProfileDropdownOpen(!isProfileDropdownOpen)}
                className="flex items-center gap-3 h-10 px-4 rounded-md border border-white/10 text-white hover:bg-white/5 transition-all active:scale-95"
              >
                <span className="material-symbols-outlined text-xl">account_circle</span>
                <span className="hidden md:block text-[10px] font-black uppercase tracking-widest max-w-[80px] truncate">{user}</span>
                <span className="material-symbols-outlined text-xs opacity-40">expand_more</span>
              </button>
              
              <AnimatePresence>
                {isProfileDropdownOpen && (
                  <motion.div 
                    initial={{ opacity: 0, scale: 0.95, y: 5 }}
                    animate={{ opacity: 1, scale: 1, y: 0 }}
                    exit={{ opacity: 0, scale: 0.95, y: 5 }}
                    transition={{ duration: 0.2 }}
                    className="absolute right-0 top-full mt-3 w-56 bg-card-darker border border-white/10 rounded-lg shadow-2xl overflow-hidden"
                  >
                    <div className="px-5 py-4 border-b border-white/5 bg-white/2">
                      <p className="text-[9px] uppercase font-black text-primary tracking-[0.3em] mb-1 opacity-60">{t["auth-welcome"]}</p>
                      <p className="text-xs font-black text-white truncate">{user}</p>
                    </div>
                    <div className="p-1">
                      <Link href="/hesabim" className="flex items-center gap-4 px-4 py-3 text-[10px] font-black uppercase tracking-widest text-white/60 hover:text-white hover:bg-white/5 transition-all">
                        <span className="material-symbols-outlined text-lg">settings</span>
                        {t["nav-profile"]}
                      </Link>
                      <button 
                        onClick={handleLogout}
                        className="flex items-center gap-4 w-full px-4 py-3 text-[10px] font-black uppercase tracking-widest text-red-500/60 hover:text-white hover:bg-red-500 transition-all"
                      >
                        <span className="material-symbols-outlined text-lg">logout</span>
                        {t["auth-logout"]}
                      </button>
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>
          )}

          {/* Mobile Menu Btn */}
          <button 
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            className="lg:hidden w-10 h-10 flex items-center justify-center rounded-lg border border-white/10 text-white transition-all active:scale-95"
          >
            <span className="material-symbols-outlined text-2xl">{isMobileMenuOpen ? 'close' : 'menu'}</span>
          </button>
        </div>
      </div>

      {/* Mobile Nav */}
      <AnimatePresence>
        {isMobileMenuOpen && (
          <motion.div 
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
            className="lg:hidden absolute top-full left-0 right-0 bg-background-dark border-b border-white/10 shadow-2xl z-[90]"
          >
            <div className="p-6 flex flex-col gap-4">
              <Link href="/pts" onClick={() => setIsMobileMenuOpen(false)} className="px-5 py-4 bg-white/5 rounded-lg text-xs font-black uppercase tracking-widest">{t["nav-wizard"]}</Link>
              <Link href="/puk" onClick={() => setIsMobileMenuOpen(false)} className="px-5 py-4 bg-white/5 rounded-lg text-xs font-black uppercase tracking-widest">{t["nav-compatibility"]}</Link>
              <Link href="/pbg" onClick={() => setIsMobileMenuOpen(false)} className="px-5 py-4 bg-white/5 rounded-lg text-xs font-black uppercase tracking-widest">{t["nav-info"]}</Link>
              <div className="grid grid-cols-2 gap-4 mt-2">
                <button onClick={() => toggleLang('tr')} className={`py-4 rounded-lg font-black text-[10px] tracking-widest border transition-all ${lang === 'tr' ? 'bg-primary border-primary text-white' : 'border-white/10 text-white/40'}`}>TR</button>
                <button onClick={() => toggleLang('en')} className={`py-4 rounded-lg font-black text-[10px] tracking-widest border transition-all ${lang === 'en' ? 'bg-primary border-primary text-white' : 'border-white/10 text-white/40'}`}>EN</button>
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </nav>
  );
}

