"use client";
import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';
import Link from 'next/link';

export default function ForgotPasswordPage() {
  const router = useRouter();
  const [step, setStep] = useState(1); // 1: Email, 2: Code
  const [email, setEmail] = useState('');
  const [code, setCode] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSendCode = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await api.post('/auth/forgot-password', { email });
      if (response.ok) {
        setStep(2);
        Swal.fire({
          title: 'Kod Gönderildi',
          text: 'E-posta adresinizi kontrol edin.',
          icon: 'success',
          background: '#1f2937', color: '#fff'
        });
      } else {
        const result = await response.json();
        Swal.fire({ title: 'Hata', text: result.message || 'Kod gönderilemedi.', icon: 'error', background: '#1f2937', color: '#fff' });
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyCode = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await api.post('/auth/verify-code', { email, code });
      const result = await response.json();

      if (response.ok && result.token) {
        // Redirect to new password page (we'll implement this next)
        router.push(`/sifre-sifirla?email=${encodeURIComponent(email)}&token=${encodeURIComponent(result.token)}`);
      } else {
        Swal.fire({ title: 'Hata', text: result.message || 'Kod hatalı.', icon: 'error', background: '#1f2937', color: '#fff' });
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen flex items-center justify-center p-6 sm:p-12 overflow-hidden bg-black">
      {/* Background Video (Mockup/Link) */}
      <video autoPlay loop muted playsInline className="absolute inset-0 w-full h-full object-cover opacity-40 grayscale">
        <source src="/Videos/Video1.webm" type="video/webm" />
      </video>
      <div className="absolute inset-0 bg-gradient-to-t from-black via-transparent to-black/50"></div>

      <div className="relative w-full max-w-md bg-card-darker/90 backdrop-blur-xl p-10 rounded-3xl border border-white/10 shadow-2xl animate-fade-in-down">
        <Link href="/" className="absolute -top-16 left-1/2 -translate-x-1/2 w-12 h-12 bg-primary flex items-center justify-center rounded-2xl shadow-xl shadow-primary/30 hover:scale-110 active:scale-95 transition-all group">
          <span className="material-symbols-outlined text-white transition-transform group-hover:-translate-y-0.5">home</span>
        </Link>

        <div className="text-center mb-10">
          <h1 className="text-3xl font-black mb-3 tracking-tight">Şifre Sıfırlama</h1>
          <p className="text-gray-400 text-sm font-medium">
            {step === 1 ? 'E-posta adresinizi girerek başlayın.' : 'E-postanıza gönderilen 6 haneli kodu girin.'}
          </p>
        </div>

        {step === 1 ? (
          <form onSubmit={handleSendCode} className="space-y-6">
            <div className="space-y-2">
              <label className="text-xs font-black text-gray-500 uppercase tracking-widest ml-1">Email</label>
              <input 
                type="email" 
                required 
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="ornek@mail.com"
                className="w-full bg-gray-900 border border-white/5 rounded-2xl px-6 py-4 text-white focus:outline-none focus:ring-2 focus:ring-primary transition-all hover:bg-gray-800"
              />
            </div>
            <button 
              type="submit" 
              disabled={loading}
              className="w-full bg-primary text-white font-black py-4 rounded-2xl shadow-xl shadow-primary/20 hover:brightness-110 active:scale-95 transition-all uppercase tracking-widest text-sm"
            >
              {loading ? 'Gönderiliyor...' : 'Kod Gönder'}
            </button>
          </form>
        ) : (
          <form onSubmit={handleVerifyCode} className="space-y-6">
            <div className="text-center mb-6">
              <span className="text-xs text-gray-500 font-bold uppercase tracking-widest block mb-1">Kod Gönderilen Adres:</span>
              <span className="text-primary font-bold">{email}</span>
            </div>
            <div className="space-y-2">
              <label className="text-xs font-black text-gray-500 uppercase tracking-widest ml-1 text-center block">Doğrulama Kodu</label>
              <input 
                type="text" 
                required 
                maxLength={6}
                value={code}
                onChange={(e) => setCode(e.target.value)}
                placeholder="******"
                className="w-full bg-gray-900 border border-white/5 rounded-2xl px-6 py-4 text-white text-center text-3xl font-black tracking-[1em] focus:outline-none focus:ring-2 focus:ring-primary transition-all hover:bg-gray-800"
              />
            </div>
            <button 
              type="submit" 
              disabled={loading}
              className="w-full bg-primary text-white font-black py-4 rounded-2xl shadow-xl shadow-primary/20 hover:brightness-110 active:scale-95 transition-all uppercase tracking-widest text-sm"
            >
              {loading ? 'Doğrulanıyor...' : 'Kodu Doğrula'}
            </button>
            <button 
              type="button"
              onClick={() => setStep(1)}
              className="w-full text-xs font-black text-gray-500 uppercase tracking-widest hover:text-white transition-colors"
            >
              E-POSTA ADRESİNİ DEĞİŞTİR
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
