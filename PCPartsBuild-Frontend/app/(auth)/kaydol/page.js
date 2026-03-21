"use client";
import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { motion, AnimatePresence } from 'framer-motion';
import { translations } from '@/lib/translations';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';

export default function RegisterPage() {
  const router = useRouter();
  const [lang, setLang] = useState('tr');
  const [formData, setFormData] = useState({
    fullName: '',
    username: '',
    email: '',
    password: '',
    repeatPassword: ''
  });
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    const savedLang = localStorage.getItem('lang') || 'tr';
    setLang(savedLang);
    const loggedInUser = localStorage.getItem('loggedInUser');
    if (loggedInUser) router.push('/');
  }, [router]);

  const t = translations[lang];

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.id]: e.target.value });
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    
    if (formData.password !== formData.repeatPassword) {
      Swal.fire({
        title: lang === 'tr' ? 'Hata' : 'Error',
        text: lang === 'tr' ? 'Şifreler eşleşmiyor!' : 'Passwords do not match!',
        icon: 'warning',
        background: '#0d1117',
        color: '#fff',
        confirmButtonColor: '#16a3b2'
      });
      return;
    }

    setIsLoading(true);

    try {
      const response = await api.post('/auth/register', {
        fullName: formData.fullName,
        username: formData.username,
        email: formData.email,
        password: formData.password
      });

      if (response.ok) {
        Swal.fire({
          title: lang === 'tr' ? 'Kayıt Başarılı!' : 'Registration Successful!',
          text: lang === 'tr' ? 'Hesabınız oluşturuldu. Giriş sayfasına yönlendiriliyorsunuz.' : 'Account created. Redirecting to login...',
          icon: 'success',
          background: '#0d1117',
          color: '#fff',
          confirmButtonColor: '#16a3b2',
          timer: 2000,
          timerProgressBar: true,
          showConfirmButton: false
        }).then(() => {
          router.push('/giris');
        });
      } else {
        const errorData = await response.json();
        let errorMessage = lang === 'tr' ? "Bir hata oluştu." : "An error occurred.";
        
        if (Array.isArray(errorData)) {
          errorMessage = errorData.map(err => `• ${err.description}`).join('\n');
        } else if (errorData.message) {
          errorMessage = errorData.message;
        }

        Swal.fire({
          title: lang === 'tr' ? 'Kayıt Başarısız' : 'Registration Failed',
          text: errorMessage,
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
    <div className="relative min-h-screen w-full flex items-center justify-center py-12 px-4 overflow-hidden bg-background-dark">
      {/* Structural Decoration */}
      <div className="fixed inset-0 z-0 text-white/5 opacity-10 pointer-events-none select-none">
        <div className="absolute inset-0 flex items-center justify-center -rotate-12 scale-150 font-black text-[20vw] leading-none tracking-tighter overflow-hidden">
           GENESIS
        </div>
      </div>
      
      <div className="fixed inset-0 z-0">
        <div className="absolute inset-0 bg-gradient-to-tl from-background-dark via-background-dark to-indigo-500/5"></div>
      </div>

      <motion.div 
        initial={{ opacity: 0, scale: 0.98 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.4, ease: "easeOut" }}
        className="relative z-10 w-full max-w-2xl px-4"
      >
        <div className="bg-card-darker border border-white/10 rounded-xl p-8 md:p-12 shadow-2xl relative overflow-hidden active:scale-[0.995] transition-transform">
          
          <div className="absolute top-0 right-0 w-32 h-32 bg-primary/5 -mr-16 -mt-16 rotate-45 border-b border-primary/20 pointer-events-none"></div>

          <header className="mb-10 text-center">
            <Link href="/" className="inline-block mb-10">
              <img src="/Images/logo.webp" alt="Logo" className="h-10 w-auto brightness-110" />
            </Link>
            <h1 className="text-3xl font-black text-white tracking-tight leading-none mb-4 uppercase">
              {lang === 'tr' ? 'Yeni Hesap' : 'New Account'}
            </h1>
            <div className="h-1 w-12 bg-primary mx-auto rounded-full mb-6"></div>
            <p className="text-white/40 text-[10px] font-black uppercase tracking-[0.2em] leading-relaxed max-w-sm mx-auto">
              {lang === 'tr' ? 'Donanım veritabanına sınırsız erişim için kimlik tanımla' : 'Define your identity for unlimited hardware database access'}
            </p>
          </header>

          <form onSubmit={handleRegister} className="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            <div className="md:col-span-2 space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary ml-1 opacity-60">
                 {lang === 'tr' ? 'Tam İsim' : 'Full Name'}
              </label>
              <input 
                type="text" id="fullName" required
                value={formData.fullName} onChange={handleChange}
                placeholder={lang === 'tr' ? 'Adınız Soyadınız' : 'John Doe'}
                className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary ml-1 opacity-60">E-Posta</label>
              <input 
                type="email" id="email" required
                value={formData.email} onChange={handleChange}
                placeholder="unit@hardware.net"
                className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary ml-1 opacity-60">{lang === 'tr' ? 'Kod Adı' : 'Code Name'}</label>
              <input 
                type="text" id="username" required
                value={formData.username} onChange={handleChange}
                placeholder="user_unit_01"
                className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary ml-1 opacity-60">{lang === 'tr' ? 'Anahtar' : 'Key'}</label>
              <div className="relative">
                <input 
                  type={showPassword ? "text" : "password"} id="password" required
                  value={formData.password} onChange={handleChange}
                  placeholder="••••••••"
                  className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 pr-12 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
                />
                <button type="button" onClick={() => setShowPassword(!showPassword)} className="absolute right-4 top-1/2 -translate-y-1/2 text-white/20 hover:text-white transition-colors">
                  <span className="material-symbols-outlined text-lg">{showPassword ? 'visibility' : 'visibility_off'}</span>
                </button>
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-primary ml-1 opacity-60">{lang === 'tr' ? 'Doğrulama' : 'Verification'}</label>
              <input 
                type={showPassword ? "text" : "password"} id="repeatPassword" required
                value={formData.repeatPassword} onChange={handleChange}
                placeholder="••••••••"
                className="w-full h-12 bg-white/2 border border-white/10 rounded px-4 text-white text-sm placeholder:text-white/20 focus:border-primary focus:bg-white/5 outline-none transition-all font-bold"
              />
            </div>

            <div className="md:col-span-2 pt-6">
              <button 
                disabled={isLoading}
                className="group relative w-full h-14 bg-primary text-white font-black text-xs uppercase tracking-[0.2em] rounded transition-all hover:brightness-110 active:scale-95 shadow-xl shadow-primary/10 flex items-center justify-center gap-3 disabled:opacity-50"
              >
                {isLoading ? (
                  <span className="material-symbols-outlined animate-spin text-sm">sync</span>
                ) : (
                  <>
                    <span className="material-symbols-outlined text-xl group-hover:translate-x-1 transition-transform">how_to_reg</span>
                    {lang === 'tr' ? 'Hesabı Aktifleştir' : 'Activate Account'}
                  </>
                )}
              </button>
            </div>
          </form>

          <footer className="mt-12 text-center pt-8 border-t border-white/5">
            <p className="text-[10px] font-black uppercase tracking-widest text-white/40">
              {lang === 'tr' ? 'Zaten tanımlı mısın?' : 'Already defined?'}{' '}
              <Link href="/giris" className="text-primary hover:underline underline-offset-4 ml-2">
                {lang === 'tr' ? 'Giriş Yap' : 'Log In'}
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
