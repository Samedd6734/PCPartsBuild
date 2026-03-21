"use client";
import React, { useEffect, useState } from 'react';
import Link from 'next/link';
import { translations } from '@/lib/translations';

export default function Footer() {
  const [lang, setLang] = useState('tr');

  useEffect(() => {
    const savedLang = localStorage.getItem('lang') || 'tr';
    setLang(savedLang);
    
    const handleStorageChange = () => {
      setLang(localStorage.getItem('lang') || 'tr');
    };
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  const t = translations[lang];

  const socialLinks = [
    { name: 'Twitter', href: '#', icon: (
      <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 24 24"><path d="M24 4.557c-.883.392-1.832.656-2.828.775 1.017-.609 1.798-1.574 2.165-2.724-.951.564-2.005.974-3.127 1.195-.897-.957-2.178-1.555-3.594-1.555-3.179 0-5.515 2.966-4.797 6.045-4.091-.205-7.719-2.165-10.148-5.144-1.29 2.213-.669 5.108 1.523 6.574-.806-.026-1.566-.247-2.229-.616-.054 2.281 1.581 4.415 3.949 4.89-.693.188-1.452.232-2.224.084.626 1.956 2.444 3.379 4.6 3.419-2.07 1.623-4.678 2.348-7.29 2.04 2.179 1.397 4.768 2.212 7.548 2.212 9.142 0 14.307-7.721 13.995-14.646.962-.695 1.797-1.562 2.457-2.549z"/></svg>
    )},
    { name: 'Facebook', href: '#', icon: (
      <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 24 24"><path fillRule="evenodd" d="M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.988C18.343 21.128 22 16.991 22 12z" clipRule="evenodd"></path></svg>
    )},
    { name: 'Instagram', href: '#', icon: (
      <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 24 24"><path fillRule="evenodd" d="M12.315 2c2.43 0 2.784.013 3.808.06 1.064.049 1.791.218 2.427.465a4.902 4.902 0 011.772 1.153 4.902 4.902 0 011.153 1.772c.247.636.416 1.363.465 2.427.048 1.024.06 1.378.06 3.808s-.012 2.784-.06 3.808c-.049 1.064-.218 1.791-.465 2.427a4.902 4.902 0 01-1.153 1.772 4.902 4.902 0 01-1.772 1.153c-.636.247-1.363.416-2.427.465-1.024.048-1.378.06-3.808.06s-2.784-.013-3.808-.06c-1.064-.049-1.791-.218-2.427-.465a4.902 4.902 0 01-1.772-1.153 4.902 4.902 0 01-1.153-1.772c-.247-.636-.416-1.363-.465-2.427-.048-1.024-.06-1.378-.06-3.808s.012-2.784.06-3.808c.049-1.064.218-1.791.465-2.427a4.902 4.902 0 011.153-1.772A4.902 4.902 0 016.345 2.525c.636-.247 1.363-.416 2.427-.465C9.793 2.013 10.147 2 12.315 2zm0 1.623c-2.403 0-2.72.01-3.667.058-1.146.052-1.602.248-1.922.458-.426.28-.765.624-1.043 1.043-.28.426-.624.765-1.043 1.043-.21.32-.406.776-.458 1.922-.048.947-.058 1.264-.058 3.667s.01 2.72.058 3.667c.052 1.146.248 1.602.458 1.922.28.426.624.765 1.043 1.043.426.28.765.624 1.043 1.043.32.21.776.406 1.922.458.947.048 1.264.058 3.667.058s2.72-.01 3.667-.058c1.146-.052 1.602-.248 1.922-.458.426-.28.765-.624 1.043-1.043.28-.426.624-.765 1.043-1.043.32-.21.406-.776.458-1.922.048-.947.058-1.264.058-3.667s-.01-2.72-.058-3.667c-.052-1.146-.248-1.602-.458-1.922-.28-.426-.624-.765-1.043-1.043-.426-.28-.765-.624-1.043-1.043-.32-.21-.776-.406-1.922-.458-.947-.048-1.264-.058-3.667-.058zM12 6.865a5.135 5.135 0 100 10.27 5.135 5.135 0 000-10.27zm0 1.623a3.512 3.512 0 110 7.024 3.512 3.512 0 010-7.024zM16.536 6.808a1.2 1.2 0 100 2.4 1.2 1.2 0 000-2.4z" clipRule="evenodd"></path></svg>
    )},
  ];

  return (
    <footer className="w-full bg-background-dark border-t border-white/5 pt-16 pb-8 mt-auto overflow-hidden">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          
          {/* Brand Info */}
          <div className="md:col-span-2 flex flex-col gap-6">
            <Link href="/" className="group inline-flex items-center">
              <img src="/Images/logo.webp" alt="Logo" className="h-8 w-auto brightness-110 group-hover:scale-105 transition-transform duration-300" />
            </Link>
            <p className="text-white/40 text-sm leading-relaxed max-w-sm">
              {lang === 'tr' 
                ? 'Donanım uyumluluğu dertlerini bir kenara bırakın. PCPartsBuild ile hayalinizdeki sistemi profesyonel araçlarımızla saniyeler içinde tasarlayın.' 
                : 'Leave hardware compatibility worries behind. Design your dream system in seconds with PCPartsBuild professional tools.'}
            </p>
            <div className="flex items-center gap-4">
              {socialLinks.map((social) => (
                <Link 
                  key={social.name} 
                  href={social.href} 
                  className="w-10 h-10 rounded-xl bg-white/5 flex items-center justify-center text-white/40 hover:text-primary hover:bg-primary/10 transition-all border border-white/5 active:scale-90"
                >
                  {social.icon}
                </Link>
              ))}
            </div>
          </div>

          {/* Quick Links */}
          <div className="flex flex-col gap-6">
            <h4 className="text-white text-xs font-black uppercase tracking-[0.2em]">{lang === 'tr' ? 'Keşfet' : 'Explore'}</h4>
            <ul className="flex flex-col gap-3">
              <li><Link href="/pts" className="text-white/40 hover:text-white text-sm font-medium transition-colors">{t["nav-wizard"]}</Link></li>
              <li><Link href="/puk" className="text-white/40 hover:text-white text-sm font-medium transition-colors">{t["nav-compatibility"]}</Link></li>
              <li><Link href="/pbg" className="text-white/40 hover:text-white text-sm font-medium transition-colors">{t["nav-info"]}</Link></li>
            </ul>
          </div>

          {/* Support Info */}
          <div className="flex flex-col gap-6">
            <h4 className="text-white text-xs font-black uppercase tracking-[0.2em]">{lang === 'tr' ? 'İletişim' : 'Contact'}</h4>
            <ul className="flex flex-col gap-3">
              <li className="flex flex-col">
                <span className="text-white/20 text-[10px] font-black uppercase tracking-widest leading-none mb-1">E-Posta</span>
                <a href="mailto:pcpartsbuild@gmail.com" className="text-white/40 hover:text-primary text-sm font-medium transition-colors truncate">pcpartsbuild@gmail.com</a>
              </li>
              <li className="flex flex-col">
                <span className="text-white/20 text-[10px] font-black uppercase tracking-widest leading-none mb-1">Destek</span>
                <span className="text-white/40 text-sm font-medium">{lang === 'tr' ? 'Sıkça Sorulan Sorular' : 'FAQ'}</span>
              </li>
            </ul>
          </div>
        </div>

        {/* Copyright Section */}
        <div className="pt-8 border-t border-white/5 flex flex-col md:flex-row items-center justify-between gap-4">
          <p className="text-white/20 text-xs font-medium uppercase tracking-widest">
            {t["footer-copyright"]}
          </p>
          <div className="flex items-center gap-6">
            <span className="text-white/10 text-[10px] font-black uppercase tracking-[0.3em] select-none">REMASTERED 2026 EDITION</span>
          </div>
        </div>
      </div>
    </footer>
  );
}
