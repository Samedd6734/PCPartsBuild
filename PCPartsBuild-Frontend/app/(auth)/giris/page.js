"use client";
import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { motion, AnimatePresence } from 'framer-motion';
import { translations } from '@/lib/translations';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';

export default function LoginPage() {
  const router = useRouter();
  const [lang, setLang] = useState('tr');
  const [loginIdentifier, setLoginIdentifier] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    const savedLang = localStorage.getItem('lang') || 'tr';
    setLang(savedLang);
    const loggedInUser = localStorage.getItem('loggedInUser');
    if (loggedInUser) router.push('/');
  }, [router]);

  const t = translations[lang];

  const handleLogin = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const response = await api.post('/auth/login', {
        loginIdentifier,
        password
      });

      const result = await response.json();

      if (response.ok) {
        localStorage.setItem('loggedInUser', result.username);
        localStorage.setItem('token', result.token);
        if (result.userId) localStorage.setItem('userId', result.userId);

        Swal.fire({
          title: lang === 'tr' ? 'Hoş Geldiniz!' : 'Welcome!',
          text: lang === 'tr' ? 'Giriş başarılı. Yönlendiriliyorsunuz...' : 'Login successful. Redirecting...',
          icon: 'success',
          background: '#0d1117',
          color: '#fff',
          confirmButtonColor: '#16a3b2',
          timer: 1500,
          timerProgressBar: true,
          showConfirmButton: false
        }).then(() => {
          router.push('/');
        });
      } else {
        Swal.fire({
          title: lang === 'tr' ? 'Giriş Başarısız' : 'Login Failed',
          text: result.message || (lang === 'tr' ? 'Kullanıcı adı veya şifre hatalı.' : 'Invalid username or password.'),
          icon: 'error',
          background: '#0d1117',
          color: '#fff',
          confirmButtonColor: '#ff4444'
        });
      }
    } catch (error) {
      console.error('Hata:', error);
      Swal.fire({
        title: lang === 'tr' ? 'Bağlantı Hatası' : 'Connection Error',
        text: lang === 'tr' ? 'Sunucuya ulaşılamadı. Lütfen internet bağlantınızı kontrol edin.' : 'Could not reach server. Please check your internet connection.',
        icon: 'error',
        background: '#0d1117',
        color: '#fff',
        confirmButtonColor: '#ff4444'
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen w-full flex items-center justify-center overflow-hidden bg-background-dark">
      {/* Precision Background */}
      <div className="fixed inset-0 z-0 text-white/5 opacity-10 pointer-events-none select-none">
        <div className="absolute inset-0 flex items-center justify-center -rotate-12 scale-150 font-black text-[20vw] leading-none tracking-tighter overflow-hidden">
           HARDWARE
        </div>
      </div>
      
      <div className="fixed inset-0 z-0">
        <div className="absolute inset-0 bg-gradient-to-br from-background-dark via-background-dark to-primary/5"></div>
      </div>

      <motion.div 
        initial={{ opacity: 0, scale: 0.98 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.4, ease: "easeOut" }}
        className="relative z-10 w-full max-w-md px-4"
      >
        <div className="bg-card-darker border border-white/10 rounded-xl p-8 md:p-10 shadow-2xl relative overflow-hidden active:scale-[0.995] transition-transform">
          
          <div className="absolute top-0 right-0 w-24 h-24 bg-primary/5 -mr-12 -mt-12 rotate-45 border-b border-primary/20 pointer-events-none"></div>

          <header className="mb-10 text-center">
            <Link href="/" className="inline-block mb-8">
              <img src="/Images/logo.webp" alt="Logo" className="h-10 w-auto brightness-110" />
            </Link>
            <h1 className="text-3xl font-black text-white tracking-tight leading-none mb-3 uppercase">
              {lang === 'tr' ? 'Giriş Yap' : 'Log In'}
            </h1>
            <div className="h-1 w-12 bg-primary mx-auto rounded-full mb-4"></div>
            <p className="text-white/40 text-[10px] font-black uppercase tracking-widest leading-relaxed">
              {lang === 'tr' ? 'Donanım ekosistemine eriş' : 'Access the hardware ecosystem'}
            </p>
          </header>

          <form onSubmit={handleLogin} className="space-y-5">
            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary ml-1 opacity-60">
                 {lang === 'tr' ? 'Kullanıcı Kimliği' : 'User Identity'}
              </label>
              <div className="relative group">
                <input 
                  type="text"
                  required
                  value={loginIdentifier}
                  onChange={(e) => setLoginIdentifier(e.target.value)}
                  placeholder={lang === 'tr' ? 'Kullanıcı adı veya e-posta' : 'Username or email'}
                  className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
                />
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex justify-between items-center px-1">
                <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary opacity-60">
                   {lang === 'tr' ? 'Erişim Anahtarı' : 'Access Key'}
                </label>
                <Link href="/sifremi-unuttum" className="text-[9px] font-black uppercase tracking-widest text-white/20 hover:text-primary transition-colors">
                  {lang === 'tr' ? 'Şifremi Unuttum?' : 'Forgot Password?'}
                </Link>
              </div>
              <div className="relative group">
                <input 
                  type={showPassword ? "text" : "password"}
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="••••••••"
                  className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 pr-12 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
                />
                <button 
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-4 top-1/2 -translate-y-1/2 text-white/20 hover:text-white transition-colors"
                >
                  <span className="material-symbols-outlined text-lg">
                    {showPassword ? 'visibility' : 'visibility_off'}
                  </span>
                </button>
              </div>
            </div>

            <button 
              disabled={isLoading}
              className="group relative w-full h-12 bg-primary text-white font-black text-xs uppercase tracking-[0.2em] rounded transition-all hover:brightness-110 active:scale-95 shadow-lg shadow-primary/10 flex items-center justify-center gap-3 mt-6 disabled:opacity-50"
            >
              {isLoading ? (
                <span className="material-symbols-outlined animate-spin text-sm">sync</span>
              ) : (
                <>
                  <span className="material-symbols-outlined text-lg group-hover:translate-x-1 transition-transform">login</span>
                  {lang === 'tr' ? 'Sisteme Bağlan' : 'Connect System'}
                </>
              )}
            </button>
          </form>

          <footer className="mt-8 text-center pt-8 border-t border-white/5">
            <p className="text-[10px] font-black uppercase tracking-widest text-white/40">
              {lang === 'tr' ? 'Erişimin yok mu?' : "No access yet?"}{' '}
              <Link href="/kaydol" className="text-primary hover:underline underline-offset-4 ml-2">
                {lang === 'tr' ? 'Hesap Oluştur' : 'Initialize Account'}
              </Link>
            </p>
          </footer>
        </div>
      </motion.div>

      {/* Signature */}
      <div className="absolute bottom-10 left-0 right-0 text-center">
        <div className="h-px w-24 bg-white/5 mx-auto mb-4"></div>
        <p className="text-[9px] font-black uppercase tracking-[0.8em] text-white/5 select-none pointer-events-none">
          PCPARTSBUILD HARDWARE ECOSYSTEM
        </p>
      </div>
    </div>
  );
}
